using chopify.Models;

namespace chopify.Services.Interfaces
{
    public interface IWinnerService
    {
        Task<WinnerReadDTO> GetLastAsync();
    }
}