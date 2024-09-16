using chopify.Data.Entities;

namespace chopify.Data.Repositories.Interfaces
{
    public interface IVoteRepository : IGenericRepository<Vote>
    {
        Task<Vote> GetByUserAsync(string user);

        Task<IEnumerable<Vote>> GetByRoundAsync(int roundNumber);

        Task DeleteAllAsync();

        Task DeleteAllExceptAsync(IEnumerable<string> songIds);

        Task DeleteAllByRoundAsync(int roundNumber);
    }
}
