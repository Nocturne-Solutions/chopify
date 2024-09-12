using chopify.Models;

namespace chopify.Services.Interfaces
{
    public interface IVoteService
    {
        Task<VoteReadDTO> GetByUserAsync(string user);
        Task<bool> AddVoteAsync(VoteUpsertDTO vote);
    }
}
