using chopify.Data.Entities;

namespace chopify.Data.Repositories.Interfaces
{
    public interface IWinnerRepository : IGenericRepository<Winner>
    {
        Task<int> GetLastRoundNumberAsync();

        Task<Winner> GetBySongIdAsync(string songId);

        Task<Winner> GetLastAsync();
    }
}
