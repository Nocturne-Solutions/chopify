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
        private static readonly string[] invalidNames = ["admin", "administrador", "administradora", "sistema", "system"];

        private static readonly SemaphoreSlim _createSemaphore = new(1, 1);

        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<string> CreateAsync(UserUpsertDTO dto)
        {
            var normalizedName = NormalizeName(dto.Name);

            if (invalidNames.Contains(normalizedName))
                throw new ArgumentException($"El nombre no puede contener la palabra '{normalizedName}'.");

            await _createSemaphore.WaitAsync();

            try
            {
                var normalizeName = NormalizeName(dto.Name);    
                var tag = await _userRepository.GetLastTagByNormalizedName(normalizeName);

                var newUser = new User()
                {
                    Name = dto.Name,
                    NormalizedName = normalizeName,
                    Tag = tag + 1,
                    ExpireAt = DateTime.UtcNow + TimeSpan.FromHours(12)
                };

                await _userRepository.CreateAsync(newUser);

                return string.Concat(newUser.Name, "#", newUser.Tag);
            }
            finally
            {
                _createSemaphore.Release();
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
