using chopify.Data.Entities;

namespace chopify.Data.Repositories.Interfaces
{
    public interface ISongRepository : IGenericRepository<Song>
    {
        Task<IEnumerable<Song>> FetchAsync(string search);
    }
}
