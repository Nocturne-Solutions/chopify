using chopify.Configurations;
using chopify.Data.Entities;
using chopify.Data.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace chopify.Data.Repositories.Implementations
{
    public class UserRepository(IMongoClient mongoClient, IOptions<MongoDBSettings> settings) : GenericRepository<User>(mongoClient, settings), IUserRepository
    {
        private readonly IMongoCollection<User> _collection = mongoClient.GetDatabase(settings.Value.DatabaseName).GetCollection<User>("users");

        public async Task<User> GetByTagAsync(string tag) =>
            await _collection.Find(x => x.Tag.Equals(tag)).FirstOrDefaultAsync();
    }
}
