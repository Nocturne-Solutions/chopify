using chopify.Models;

namespace chopify.Services.Interfaces
{
    public interface ISuggestionService
    {
        enum ResultCodes
        {
            Success,
            SongNotFound,
            SongAlreadySuggested,
            UserAlreadySuggested,
            NoRoundInProgress
        }

        Task<IEnumerable<SuggestionReadDTO>> GetAllAsync();

        Task<ResultCodes> SuggestSong(SuggestionUpsertDTO suggestion);
    }
}
