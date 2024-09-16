using chopify.Configurations;
using chopify.Data.Entities;
using chopify.Data.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace chopify.Data.Repositories.Implementations
{
    public class VoteRepository(IMongoClient mongoClient, IOptions<MongoDBSettings> settings) : GenericRepository<Vote>(mongoClient, settings), IVoteRepository
    {
        private readonly IMongoCollection<Vote> _collection = mongoClient.GetDatabase(settings.Value.DatabaseName).GetCollection<Vote>("vote");

        public async Task DeleteAllAsync() =>
            await _collection.DeleteManyAsync(Builders<Vote>.Filter.Empty);

        public async Task DeleteAllExceptAsync(IEnumerable<string> songIds) =>
            await _collection.DeleteManyAsync(Builders<Vote>.Filter.Not(Builders<Vote>.Filter.In(x => x.SpotifySongId, songIds)));

        public async Task DeleteAllByRoundAsync(int roundNumber) =>
            await _collection.DeleteManyAsync(Builders<Vote>.Filter.Eq(x => x.VoteRoundNumber, roundNumber));

        public async Task<Vote> GetByUserAsync(string user) =>
            await _collection.Find(x => x.User.Equals(user)).FirstOrDefaultAsync();

        public async Task<IEnumerable<Vote>> GetByRoundAsync(int roundNumber) =>
            await _collection.Find(x => x.VoteRoundNumber == roundNumber).ToListAsync();
    }
}
