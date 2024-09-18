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

        public async Task AddOrRemoveVotesAsync(string songId, int votes)
        {
            var filter = Builders<Suggestion>.Filter.Eq(s => s.SpotifySongId, songId);
            var update = Builders<Suggestion>.Update.Inc(s => s.Votes, votes);

            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task AddOrRemoveVotesAsync(IDictionary<string, int> keyValuePairs)
        {
            var updates = keyValuePairs.Select(pair => new UpdateOneModel<Suggestion>(
                                                  Builders<Suggestion>.Filter.Eq(s => s.SpotifySongId, pair.Key),
                                                  Builders<Suggestion>.Update.Inc(s => s.Votes, pair.Value)));

            if (updates.Any())
                await _collection.BulkWriteAsync(updates);
        }

        public async Task DeleteAllAsync() =>
            await _collection.DeleteManyAsync(Builders<Suggestion>.Filter.Empty);

        public async Task DeleteAllExceptAsync(IEnumerable<string> songIds) =>
            await _collection.DeleteManyAsync(Builders<Suggestion>.Filter.Not(Builders<Suggestion>.Filter.In(s => s.SpotifySongId, songIds)));

        public async Task DeleteAllByRoundAsync(int roundNumber) =>
            await _collection.DeleteManyAsync(Builders<Suggestion>.Filter.Eq(s => s.SuggestedRoundNumber, roundNumber));

        public async Task<Suggestion> GetBySongIdAsync(string id) =>
            await _collection.Find(x => x.SpotifySongId.Equals(id)).FirstOrDefaultAsync();

        public async Task<IEnumerable<Suggestion>> GetBySongIdsAsync(IEnumerable<string> songIds)
        {
            var filter = Builders<Suggestion>.Filter.In(s => s.SpotifySongId, songIds);

            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<Suggestion> GetByUserAsync(string user) =>
            await _collection.Find(x => x.SuggestedBy.Equals(user, StringComparison.OrdinalIgnoreCase)).FirstOrDefaultAsync();

        public async Task<IEnumerable<Suggestion>> GetMostVotedAsync()
        {
            var maxVotes = await _collection
                .Find(_ => true)
                .SortByDescending(x => x.Votes)
                .Limit(1)
                .Project(x => x.Votes)
                .FirstOrDefaultAsync();

            if (maxVotes == 0) 
                return [];

            return await _collection.Find(Builders<Suggestion>.Filter.Eq(x => x.Votes, maxVotes)).ToListAsync();
        }

        public async Task<IEnumerable<Suggestion>> GetTopNAsync(int n)
        {
            var allSuggestions = await _collection.Find(Builders<Suggestion>.Filter.Empty)
                                                  .SortByDescending(s => s.Votes)
                                                  .ToListAsync();

            var groupedSuggestions = allSuggestions
                .GroupBy(s => s.Votes)
                .OrderByDescending(g => g.Key)
                .SelectMany(g => g)
                .ToList();
            
            var topSuggestions = groupedSuggestions
                .TakeWhile((s, index) => index < n || s.Votes == groupedSuggestions.Skip(n - 1).FirstOrDefault()?.Votes)
                .ToList();

            return topSuggestions;
        }
    }
}
