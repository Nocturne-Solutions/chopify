using chopify.Data.Entities;

namespace chopify.Data.Repositories.Interfaces
{
    public interface ICooldownRepository : IGenericRepository<Cooldown>
    {
        Task<Cooldown> GetBySongIdAsync(string songId);

        Task<IEnumerable<Cooldown>> GetBySongIdsAsync(IEnumerable<string> songIds);

        Task DeleteAllAsync();

        Task DeleteAllExpiredAsync();
    }
}
