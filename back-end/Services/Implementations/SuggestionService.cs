using AutoMapper;
using chopify.Data.Entities;
using chopify.Data.Repositories.Implementations;
using chopify.Data.Repositories.Interfaces;
using chopify.External;
using chopify.Models;
using chopify.Services.Interfaces;
using System.Collections.Generic;

namespace chopify.Services.Implementations
{
    public class SuggestionService(ISongRepository songRepository, ISuggestionRepository suggestionRepository, IVoteRepository voteRepository, IMapper mapper) : ISuggestionService
    {
        private static readonly SemaphoreSlim _suggestSemaphore = new(1, 1);

        private readonly ISongRepository _songRepository = songRepository;
        private readonly IVoteRepository _voteRepository = voteRepository;
        private readonly ISuggestionRepository _suggestionRepository = suggestionRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<bool> SuggestSong(SuggestionUpsertDTO suggestionDto)
        {
            var song = _mapper.Map<SongReadDTO>(await SpotifyService.Instance.GetFullTrackById(suggestionDto.SpotifySongId));

            if (song == null)
                return false;

            await _suggestSemaphore.WaitAsync();

            try
            {
                var suggestion = await _suggestionRepository.GetBySongIdAsync(suggestionDto.SpotifySongId);

                if (suggestion == null)
                {
                    var newSuggestion = new Suggestion
                    {
                        SpotifySongId = suggestionDto.SpotifySongId,
                        Artist = song.Artist,
                        Duration = song.Duration,
                        CoverUrl = song.CoverUrl,
                        FirstReleaseDate = song.FirstReleaseDate,
                        Name = song.Name,
                        Votes = 1,
                        SuggestedBy = suggestionDto.SuggestedBy
                    };

                    var newVote = new Vote
                    {
                        SpotifySongId = suggestionDto.SpotifySongId,
                        User = suggestionDto.SuggestedBy
                    };

                    await _suggestionRepository.CreateAsync(newSuggestion);
                    await _voteRepository.CreateAsync(newVote);

                    return true;
                }
                else
                    return false;
            }
            finally
            {
                _suggestSemaphore.Release();
            }
        }

        public async Task<IEnumerable<SuggestionReadDTO>> GetAllAsync()
        {
            var entities = await _suggestionRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<SuggestionReadDTO>>(entities);
        }
    }
}
