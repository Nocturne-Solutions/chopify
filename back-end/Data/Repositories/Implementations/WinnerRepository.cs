using chopify.Configurations;
using chopify.Data.Entities;
using chopify.Data.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace chopify.Data.Repositories.Implementations
{
    public class WinnerRepository(IMongoClient mongoClient, IOptions<MongoDBSettings> settings) : GenericRepository<Winner>(mongoClient, settings), IWinnerRepository
    {
        private readonly IMongoCollection<Winner> _collection = mongoClient.GetDatabase(settings.Value.DatabaseName).GetCollection<Winner>("winner");

        public async Task<int> GetLastRoundNumberAsync() =>
            (await _collection.Find(_ => true).SortByDescending(x => x.CreatedAt).FirstOrDefaultAsync())?.RoundNumber ?? 0;

        public async Task<Winner> GetBySongIdAsync(string songId) =>
            await _collection.Find(x => x.SpotifySongId == songId).FirstOrDefaultAsync();

        public async Task<Winner> GetLastAsync() =>
            await _collection.Find(_ => true).SortByDescending(x => x.CreatedAt).FirstOrDefaultAsync();
    }
}
