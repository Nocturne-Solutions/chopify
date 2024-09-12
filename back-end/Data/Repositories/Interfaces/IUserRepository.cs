using chopify.Data.Entities;

namespace chopify.Data.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<int> GetLastTagByNormalizedName(string normalizedName);
    }
}
