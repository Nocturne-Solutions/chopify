using chopify.Models;

namespace chopify.Services.Interfaces
{
    public interface ICooldownService
    {
        Task<IEnumerable<CooldownReadDTO>> GetAllAsync();
    }
}
