using chopify.Configurations;
using chopify.Data.Entities;
using chopify.Data.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace chopify.Data.Repositories.Implementations
{
    public abstract class GenericRepository<T>(IMongoClient mongoClient, IOptions<MongoDBSettings> settings) : IGenericRepository<T> where T : IEntity
    {
        private readonly IMongoCollection<T> _collection = mongoClient.GetDatabase(settings.Value.DatabaseName).GetCollection<T>(typeof(T).Name.ToLower());

        public async Task<IEnumerable<T>> GetAllAsync() =>
            await _collection.Find(_ => true).ToListAsync();

        public async Task<T> GetByIdAsync(ObjectId id) =>
            await _collection.Find(x => x.Id.Equals(id)).FirstOrDefaultAsync();

        public async Task CreateAsync(T entity) =>
            await _collection.InsertOneAsync(entity);

        public async Task UpdateAsync(ObjectId id, T entity) =>
            await _collection.ReplaceOneAsync(x => x.Id.Equals(id), entity);

        public async Task DeleteAsync(ObjectId id) =>
            await _collection.DeleteOneAsync(x => x.Id.Equals(id));
    }
}
