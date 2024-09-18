using chopify.Models;

namespace chopify.Services.Interfaces
{
    public interface IVotingSystemService
    {
        enum ResultCode
        {
            Success,
            IsActive,
            IsNotActive,
            FailToGetFirstSong
        }

        Task<ResultCode> Start();

        Task<ResultCode> Stop();

        Task<ResultCode> Reset();

        Task<VotingSystemStatusDTO> GetStatus();

        Task <int> GetCurrentRoundNumber();

        Task<bool> Lock();

        Task Unlock();
    }
}
