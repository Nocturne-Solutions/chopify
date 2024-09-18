using AutoMapper;
using chopify.Data.Repositories.Interfaces;
using chopify.Models;
using chopify.Services.Interfaces;

namespace chopify.Services.Implementations
{
    public class CooldownService(ICooldownRepository cooldownRepository, IMapper mapper) : ICooldownService
    {
        private readonly ICooldownRepository _cooldownRepository = cooldownRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<CooldownReadDTO>> GetAllAsync() =>
            _mapper.Map<IEnumerable<CooldownReadDTO>>(await _cooldownRepository.GetAllAsync());

        public async Task RecolectGarbageAsync() =>
            await _cooldownRepository.DeleteAllExpiredAsync();
    }
}
