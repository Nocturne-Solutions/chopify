using chopify.Models;

namespace chopify.Services.Interfaces
{
    public interface IVoteService
    {
        enum ResultCode
        {
            Success,
            UserAlreadyVoted,
            SongNotSuggested,
            NoRoundInProgress
        }

        Task<VoteReadDTO> GetByUserAsync(string user);
        Task<ResultCode> AddVoteAsync(VoteUpsertDTO vote);
    }
}
