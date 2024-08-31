using chopify.Data.Entities;
using MongoDB.Bson;

namespace chopify.Data.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetByTagAsync(string tag);
    }
}
