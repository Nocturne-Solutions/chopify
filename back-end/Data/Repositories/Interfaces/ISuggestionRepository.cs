using chopify.Data.Entities;

namespace chopify.Data.Repositories.Interfaces
{
    public interface ISuggestionRepository : IGenericRepository<Suggestion>
    {
        Task<Suggestion> GetBySongIdAsync(string id);

        Task<IEnumerable<Suggestion>> GetBySongIdsAsync(IEnumerable<string> songIds);
    }
}
