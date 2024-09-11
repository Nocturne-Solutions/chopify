using AutoMapper;
using chopify.Data.Entities;
using chopify.Data.Repositories.Interfaces;
using chopify.Models;
using chopify.Services.Interfaces;

namespace chopify.Services.Implementations
{
    public class SuggestionService(ISongRepository songRepository, ISuggestionRepository suggestionRepository, IMapper mapper) : ISuggestionService
    {
        private static readonly SemaphoreSlim _suggestSemaphore = new(1, 1);

        private readonly ISongRepository _songRepository = songRepository;
        private readonly ISuggestionRepository _suggestionRepository = suggestionRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<bool> SuggestSong(SuggestionDTO suggestionDto)
        {
            await _suggestSemaphore.WaitAsync();

            try
            {
                var suggestion = await _suggestionRepository.GetBySongIdAsync(suggestionDto.SpotifySongId);

                if (suggestion == null)
                {
                    await _suggestionRepository.CreateAsync(_mapper.Map<Suggestion>(suggestionDto));

                    return true;
                }
                else
                    return false;
            }
            finally
            {
                _suggestSemaphore.Release();
            }
        }
    }
}
