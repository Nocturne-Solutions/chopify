using chopify.Configurations;
using chopify.Data.Entities;
using chopify.Data.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace chopify.Data.Repositories.Implementations
{
    public class SuggestionRepository(IMongoClient mongoClient, IOptions<MongoDBSettings> settings) : GenericRepository<Suggestion>(mongoClient, settings), ISuggestionRepository
    {
        private readonly IMongoCollection<Suggestion> _collection = mongoClient.GetDatabase(settings.Value.DatabaseName).GetCollection<Suggestion>("suggestion");

        public async Task<Suggestion> GetBySongIdAsync(string id) =>
            await _collection.Find(x => x.SpotifySongId.Equals(id)).FirstOrDefaultAsync();

        public async Task<IEnumerable<Suggestion>> GetBySongIdsAsync(IEnumerable<string> songIds)
        {
            var filter = Builders<Suggestion>.Filter.In(s => s.SpotifySongId, songIds);

            return await _collection.Find(filter).ToListAsync();
        }

        public Task<Suggestion> GetByUserAsync(string user) =>
            _collection.Find(x => x.SuggestedBy.Equals(user)).FirstOrDefaultAsync();
    }
}
