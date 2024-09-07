using chopify.Data.Entities;

namespace chopify.Data.Repositories.Interfaces
{
    public interface IMusicRepository : IGenericRepository<Music>
    {
        Task<IEnumerable<Music>> FetchAsync(string search);
    }
}
