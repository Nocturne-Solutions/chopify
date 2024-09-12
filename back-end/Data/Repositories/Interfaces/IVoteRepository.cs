using chopify.Data.Entities;

namespace chopify.Data.Repositories.Interfaces
{
    public interface IVoteRepository : IGenericRepository<Vote>
    {
        Task<Vote> GetByUserAsync(string user);
    }
}
