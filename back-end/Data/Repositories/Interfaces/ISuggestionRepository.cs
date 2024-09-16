using chopify.Data.Entities;

namespace chopify.Data.Repositories.Interfaces
{
    public interface ISuggestionRepository : IGenericRepository<Suggestion>
    {
        Task<Suggestion> GetByUserAsync(string user);

        Task<Suggestion> GetBySongIdAsync(string id);

        Task<IEnumerable<Suggestion>> GetBySongIdsAsync(IEnumerable<string> songIds);

        Task<IEnumerable<Suggestion>> GetMostVotedAsync();

        Task<IEnumerable<Suggestion>> GetTopNAsync(int n);

        Task AddOrRemoveVotesAsync(string songId, int votes);

        Task AddOrRemoveVotesAsync(IDictionary<string, int> keyValuePairs);

        Task DeleteAllAsync();

        Task DeleteAllExceptAsync(IEnumerable<string> songIds);

        Task DeleteAllByRoundAsync(int roundNumber);
    }
}
