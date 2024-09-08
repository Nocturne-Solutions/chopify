using chopify.Configurations;
using chopify.Data.Entities;
using chopify.Data.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace chopify.Data.Repositories.Implementations
{
    public class MusicRepository(IMongoClient mongoClient, IOptions<MongoDBSettings> settings) : GenericRepository<Music>(mongoClient, settings), IMusicRepository
    {
        private readonly IMongoCollection<Music> _collection = mongoClient.GetDatabase(settings.Value.DatabaseName).GetCollection<Music>("music");

        public async Task<IEnumerable<Music>> FetchAsync(string search) =>
            await _collection.Find(x => x.Name.Contains(search, StringComparison.CurrentCultureIgnoreCase) || x.Artist.Contains(search, StringComparison.CurrentCultureIgnoreCase)).ToListAsync();
    }
}
