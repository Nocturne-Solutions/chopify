using chopify.Data.Entities;

namespace chopify.Data.Repositories.Interfaces
{
    public interface ISuggestionRepository : IGenericRepository<Suggestion>
    {
        Task<Suggestion?> GetBySongIdAsync(string id);

        Task<bool> IsSuggested(string id);
    }
}
