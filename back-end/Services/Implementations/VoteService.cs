using AutoMapper;
using chopify.Data.Entities;
using chopify.Data.Repositories.Interfaces;
using chopify.Models;
using chopify.Services.Interfaces;

namespace chopify.Services.Implementations
{
    public class VoteService(IVoteRepository voteRepository, ISuggestionRepository suggestionRepository, IMapper mapper) : IVoteService
    {
        private static readonly SemaphoreSlim _voteSemaphore = new(1, 1);

        private readonly IVoteRepository _voteRepository = voteRepository;
        private readonly ISuggestionRepository _suggestionRepository = suggestionRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<IVoteService.ResultCode> AddVoteAsync(VoteUpsertDTO vote)
        {
            await _voteSemaphore.WaitAsync();
            
            try
            {
                var _vote = await _voteRepository.GetByUserAsync(vote.User);

                if (_vote != null)
                    return IVoteService.ResultCode.UserAlreadyVoted;

                var suggestion = await _suggestionRepository.GetBySongIdAsync(vote.Id);

                if (suggestion == null)
                    return IVoteService.ResultCode.SongNotSuggested;

                suggestion.Votes += 1;

                await _voteRepository.CreateAsync(_mapper.Map<Vote>(vote));
                await _suggestionRepository.UpdateAsync(suggestion.Id, suggestion);

                return IVoteService.ResultCode.Success;
            }
            finally
            {
                _voteSemaphore.Release();
            }
        }

        public async Task<VoteReadDTO> GetByUserAsync(string user) =>
            _mapper.Map<VoteReadDTO>(await _voteRepository.GetByUserAsync(user));
    }
}
