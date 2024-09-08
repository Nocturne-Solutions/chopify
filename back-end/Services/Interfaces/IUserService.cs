using chopify.Models;

namespace chopify.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserReadDTO>> GetAllAsync();
        Task<UserReadDTO> GetByIdAsync(string id);
        Task CreateAsync(UserUpsertDTO dto);
        Task UpdateAsync(string id, UserUpsertDTO dto);
        Task DeleteAsync(string id);
    }
}
