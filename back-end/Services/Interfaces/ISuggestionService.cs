using chopify.Models;

namespace chopify.Services.Interfaces
{
    public interface ISuggestionService
    {
        Task<bool> SuggestSong(SuggestionDTO suggestion);
    }
}
