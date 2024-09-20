using chopify.Configurations;
using chopify.Data.Entities;
using chopify.Data.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace chopify.Data.Repositories.Implementations
{
    public class CooldownRepository(IMongoClient mongoClient, IOptions<MongoDBSettings> settings) : GenericRepository<Cooldown>(mongoClient, settings), ICooldownRepository
    {
        private readonly IMongoCollection<Cooldown> _collection = mongoClient.GetDatabase(settings.Value.DatabaseName).GetCollection<Cooldown>("cooldown");

        public override async Task<IEnumerable<Cooldown>> GetAllAsync() =>
            await _collection.Find(Builders<Cooldown>.Filter.Lt(s => s.CooldownEnd, DateTime.UtcNow)).ToListAsync();

        public async Task DeleteAllAsync() =>
            await _collection.DeleteManyAsync(FilterDefinition<Cooldown>.Empty);

        public async Task<Cooldown> GetBySongIdAsync(string songId) =>
            await _collection.Find(x => x.SpotifySongId == songId).FirstOrDefaultAsync();

        public async Task<IEnumerable<Cooldown>> GetBySongIdsAsync(IEnumerable<string> songIds) =>
            await _collection.Find(Builders<Cooldown>.Filter.In(s => s.SpotifySongId, songIds)).ToListAsync();

        public async Task DeleteAllExpiredAsync() =>
            await _collection.DeleteManyAsync(Builders<Cooldown>.Filter.Lt(s => s.CooldownEnd, DateTime.UtcNow));
    }
}
