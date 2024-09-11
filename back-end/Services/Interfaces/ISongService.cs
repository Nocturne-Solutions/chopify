using chopify.Models;

namespace chopify.Services.Interfaces
{
    public interface ISongService
    {
        Task<IEnumerable<SongReadDTO>> FetchAsync(string search);
        Task<IEnumerable<SongReadDTO>> GetMostPopularSongsArgentinaAsync();
    }
}
