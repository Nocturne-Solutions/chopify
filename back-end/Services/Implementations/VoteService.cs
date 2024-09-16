using AutoMapper;
using chopify.Data.Entities;
using chopify.Data.Repositories.Interfaces;
using chopify.Models;
using chopify.Services.Interfaces;

namespace chopify.Services.Implementations
{
    public class VoteService(IVoteRepository voteRepository, ISuggestionRepository suggestionRepository, IVotingSystemService votingRoundService, IMapper mapper) : IVoteService
    {
        private static readonly SemaphoreSlim _voteSemaphore = new(1, 1);

        private readonly IVoteRepository _voteRepository = voteRepository;
        private readonly ISuggestionRepository _suggestionRepository = suggestionRepository;
        private readonly IVotingSystemService _votingRoundService = votingRoundService;
        private readonly IMapper _mapper = mapper;

        public async Task<IVoteService.ResultCode> AddVoteAsync(VoteUpsertDTO vote)
        {
            if (!await _votingRoundService.Lock())
                return IVoteService.ResultCode.NoRoundInProgress;

            await _voteSemaphore.WaitAsync();
            
            try
            {
                var _vote = await _voteRepository.GetByUserAsync(vote.User);

                if (_vote != null)
                    return IVoteService.ResultCode.UserAlreadyVoted;

                var suggestion = await _suggestionRepository.GetBySongIdAsync(vote.Id);

                if (suggestion == null)
                    return IVoteService.ResultCode.SongNotSuggested;

                var voteEntity = _mapper.Map<Vote>(vote);

                voteEntity.VoteRoundNumber = await _votingRoundService.GetCurrentRoundNumber();

                await _voteRepository.CreateAsync(voteEntity);
                await _suggestionRepository.AddOrRemoveVotesAsync(vote.Id, 1);

                return IVoteService.ResultCode.Success;
            }
            finally
            {
                await _votingRoundService.Unlock();
                _voteSemaphore.Release();
            }
        }

        public async Task<VoteReadDTO> GetByUserAsync(string user) =>
            _mapper.Map<VoteReadDTO>(await _voteRepository.GetByUserAsync(user));
    }
}
