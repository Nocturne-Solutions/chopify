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

        public async Task<bool> AddVoteAsync(VoteUpsertDTO vote)
        {
            await _voteSemaphore.WaitAsync();
            
            try
            {
                var iAlreadyVote = await _voteRepository.GetByUserAsync(vote.User);

                if (iAlreadyVote != null)
                    return false;

                var sugestion = await _suggestionRepository.GetBySongIdAsync(vote.Id);

                if (sugestion == null)
                    return false;

                sugestion.Votes += 1;

                await _voteRepository.CreateAsync(_mapper.Map<Vote>(vote));
                await _suggestionRepository.UpdateAsync(sugestion.Id, sugestion);

                return true;
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
