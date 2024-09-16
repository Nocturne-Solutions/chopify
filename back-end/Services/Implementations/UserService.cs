using AutoMapper;
using chopify.Data.Entities;
using chopify.Data.Repositories.Interfaces;
using chopify.Models;
using chopify.Services.Interfaces;
using MongoDB.Bson;
using System.Globalization;
using System.Text;

namespace chopify.Services.Implementations
{
    public class UserService(IUserRepository userRepository, IMapper mapper) : IUserService
    {
        private static readonly SemaphoreSlim _createSemaphore = new(1, 1);
        private static readonly SemaphoreSlim _updateSemaphore = new(1, 1);

        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<string> CreateAsync(UserUpsertDTO dto)
        {
            if (dto.Name.Contains("sistema", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("El nombre no puede contener la palabra 'sistema'.", nameof(dto.Name));

            await _createSemaphore.WaitAsync();

            try
            {
                var normalizeName = NormalizeName(dto.Name);    
                var tag = await _userRepository.GetLastTagByNormalizedName(normalizeName);

                var newUser = new User()
                {
                    Name = dto.Name,
                    NormalizedName = normalizeName,
                    Tag = tag + 1
                };

                await _userRepository.CreateAsync(newUser);

                return string.Concat(newUser.Name, "#", newUser.Tag);
            }
            finally
            {
                _createSemaphore.Release();
            }
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

        public async Task<string> UpdateAsync(string id, UserUpsertDTO dto)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                throw new ArgumentException("El formato del ID no es válido.", nameof(id));

            var user = await _userRepository.GetByIdAsync(objectId);

            if (user == null)
                throw new KeyNotFoundException($"No se encontró ningún documento con el ID '{id}'.");

            await _updateSemaphore.WaitAsync();

            try
            {
                var normalizeName = NormalizeName(dto.Name);
                var tag = await _userRepository.GetLastTagByNormalizedName(normalizeName);

                user.Name = dto.Name;
                user.NormalizedName = normalizeName;
                user.Tag = tag + 1;

                await _userRepository.UpdateAsync(objectId, user);

                return string.Concat(user.Name, "#", user.Tag);
            }
            finally
            {
                _updateSemaphore.Release();
            }           
        }

        private static string NormalizeName(string name)
        {
            string formD = name.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new();

            foreach (char ch in formD)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(ch);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC).ToLower();
        }
    }
}
