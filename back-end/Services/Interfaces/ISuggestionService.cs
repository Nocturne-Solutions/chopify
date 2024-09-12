using chopify.Models;

namespace chopify.Services.Interfaces
{
    public interface ISuggestionService
    {
        Task<IEnumerable<SuggestionReadDTO>> GetAllAsync();

        Task<bool> SuggestSong(SuggestionUpsertDTO suggestion);
    }
}
