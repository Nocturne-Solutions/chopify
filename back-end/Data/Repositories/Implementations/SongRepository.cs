using chopify.Configurations;
using chopify.Data.Entities;
using chopify.Data.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace chopify.Data.Repositories.Implementations
{
    public class SongRepository(IMongoClient mongoClient, IOptions<MongoDBSettings> settings) : GenericRepository<Song>(mongoClient, settings), ISongRepository
    {
        private readonly IMongoCollection<Song> _collection = mongoClient.GetDatabase(settings.Value.DatabaseName).GetCollection<Song>("song");

        public async Task<IEnumerable<Song>> FetchAsync(string search) =>
            await _collection.Find(x => x.Name.Contains(search, StringComparison.CurrentCultureIgnoreCase) || x.Artist.Contains(search, StringComparison.CurrentCultureIgnoreCase)).ToListAsync();
    }
}
