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

        public async Task<Vote> GetByUserAsync(string user) =>
            await _collection.Find(x => x.User.Equals(user)).FirstOrDefaultAsync();
    }
}
