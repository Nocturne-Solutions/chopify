using AutoMapper;
using chopify.Data.Entities;
using chopify.Data.Repositories.Interfaces;
using chopify.External;
using chopify.Models;
using chopify.Services.Interfaces;

namespace chopify.Services.Implementations
{
    public class SuggestionService(ISongRepository songRepository, ISuggestionRepository suggestionRepository, IVoteRepository voteRepository, 
                                  IVotingSystemService votingRoundService, IMapper mapper) : ISuggestionService
    {
        private static readonly SemaphoreSlim _suggestSemaphore = new(1, 1);

        private readonly ISongRepository _songRepository = songRepository;
        private readonly IVoteRepository _voteRepository = voteRepository;
        private readonly ISuggestionRepository _suggestionRepository = suggestionRepository;
        private readonly IVotingSystemService _votingRoundService = votingRoundService;
        private readonly IMapper _mapper = mapper;
 
        public async Task<ISuggestionService.ResultCodes> SuggestSong(SuggestionUpsertDTO suggestionDto)
        {
            if (!await _votingRoundService.Lock())
                return ISuggestionService.ResultCodes.NoRoundInProgress;

            var song = _mapper.Map<SongReadDTO>(await SpotifyService.Instance.GetFullTrackById(suggestionDto.SpotifySongId));

            if (song == null)
                return ISuggestionService.ResultCodes.SongNotFound;

            await _suggestSemaphore.WaitAsync();

            try
            {
                var suggestion = await _suggestionRepository.GetBySongIdAsync(suggestionDto.SpotifySongId);

                if (suggestion != null)
                    return ISuggestionService.ResultCodes.SongAlreadySuggested;

                suggestion = await _suggestionRepository.GetByUserAsync(suggestionDto.SuggestedBy);

                if (suggestion != null)
                    return ISuggestionService.ResultCodes.UserAlreadySuggested;

                var newSuggestion = new Suggestion
                {
                    SpotifySongId = suggestionDto.SpotifySongId,
                    Artist = song.Artist,
                    Duration = song.Duration,
                    CoverUrl = song.CoverUrl,
                    FirstReleaseDate = song.FirstReleaseDate,
                    Name = song.Name,
                    Votes = 1,
                    SuggestedBy = suggestionDto.SuggestedBy,
                    SuggestedRoundNumber = await _votingRoundService.GetCurrentRoundNumber()
                };

                var newVote = new Vote
                {
                    SpotifySongId = suggestionDto.SpotifySongId,
                    User = suggestionDto.SuggestedBy
                };

                await _suggestionRepository.CreateAsync(newSuggestion);
                await _voteRepository.CreateAsync(newVote);

                return ISuggestionService.ResultCodes.Success;
            }
            finally
            {
                await _votingRoundService.Unlock();
                _suggestSemaphore.Release();
            }
        }

        public async Task<IEnumerable<SuggestionReadDTO>> GetAllAsync() =>
            _mapper.Map<IEnumerable<SuggestionReadDTO>>(await _suggestionRepository.GetAllAsync());
    }
}
