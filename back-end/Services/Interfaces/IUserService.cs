using chopify.Models;

namespace chopify.Services.Interfaces
{
    public interface IUserService
    {
        Task<string> CreateAsync(UserUpsertDTO dto);
    }
}
