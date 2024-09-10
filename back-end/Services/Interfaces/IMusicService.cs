using chopify.Models;

namespace chopify.Services.Interfaces
{
    public interface IMusicService
    {
        Task<IEnumerable<MusicReadDTO>> FetchAsync(string search);
        Task<IEnumerable<MusicReadDTO>> GetMostPopularSongsArgentinaAsync();
    }
}
