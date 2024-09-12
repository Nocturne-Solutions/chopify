using chopify.Data.Entities;
using MongoDB.Bson;

namespace chopify.Data.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : IEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(ObjectId id);
        Task CreateAsync(T entity);
        Task UpdateAsync(ObjectId id, T entity);
        Task DeleteAsync(ObjectId id);
    }
}
