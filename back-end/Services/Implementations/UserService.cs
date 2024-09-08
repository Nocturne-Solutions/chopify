using AutoMapper;
using chopify.Data.Entities;
using chopify.Data.Repositories.Interfaces;
using chopify.Models;
using chopify.Services.Interfaces;
using MongoDB.Bson;

namespace chopify.Services.Implementations
{
    public class UserService(IUserRepository userRepository, IMapper mapper) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMapper _mapper = mapper;

        public async Task CreateAsync(UserUpsertDTO dto)
        {
            var tag = Guid.NewGuid().ToString("N")[..8];

            var newUser = new User()
            {
                Name = dto.Name,
                Tag = tag
            };

            await _userRepository.CreateAsync(newUser);
        }

        public async Task DeleteAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                throw new ArgumentException("El formato del ID no es válido.", nameof(id));

            await _userRepository.DeleteAsync(objectId);
        }

        public async Task<IEnumerable<UserReadDTO>> GetAllAsync()
        {
            var entities = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserReadDTO>>(entities);
        }

        public async Task<UserReadDTO> GetByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                throw new ArgumentException("El formato del ID no es válido.", nameof(id));

            var entity = await _userRepository.GetByIdAsync(objectId);

            return _mapper.Map<UserReadDTO>(entity);
        }

        public async Task UpdateAsync(string id, UserUpsertDTO dto)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                throw new ArgumentException("El formato del ID no es válido.", nameof(id));

            var user = await _userRepository.GetByIdAsync(objectId);

            if (user == null)
                throw new KeyNotFoundException($"No se encontró ningún documento con el ID '{id}'.");

            user.Name = dto.Name;

            await _userRepository.UpdateAsync(objectId, user);
        }
    }
}
