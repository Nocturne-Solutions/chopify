using AutoMapper;
using chopify.Data.Repositories.Interfaces;
using chopify.Models;
using chopify.Services.Interfaces;

namespace chopify.Services.Implementations
{
    public class WinnerService(IWinnerRepository winnerRepository, IMapper mapper) : IWinnerService
    {
        private readonly IWinnerRepository _winnerRepository = winnerRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<WinnerReadDTO> GetLastAsync() =>
            _mapper.Map<WinnerReadDTO>(await _winnerRepository.GetLastAsync());
    }
}
