using chopify.Configurations;
using chopify.Data.Entities;
using chopify.Data.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace chopify.Data.Repositories.Implementations
{
    public class UserRepository(IMongoClient mongoClient, IOptions<MongoDBSettings> settings) : GenericRepository<User>(mongoClient, settings), IUserRepository
    {
        private readonly IMongoCollection<User> _collection = mongoClient.GetDatabase(settings.Value.DatabaseName).GetCollection<User>("user");

        public async Task<int> GetLastTagByNormalizedName(string normalizedName)
        {
            var userWithLastTag = await _collection
                .Find(x => x.NormalizedName.Equals(normalizedName))
                .SortByDescending(x => x.Tag)
                .FirstOrDefaultAsync();

            if (userWithLastTag == null)
                return 0;

            return userWithLastTag.Tag;
        }

        public async Task DeleteAllExpiredAsync() =>
            await _collection.DeleteManyAsync(Builders<User>.Filter.Lt(x => x.ExpireAt, DateTime.UtcNow));
    }
}
