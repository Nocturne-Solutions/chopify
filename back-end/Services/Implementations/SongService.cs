using AutoMapper;
using chopify.Data.Repositories.Interfaces;
using chopify.External;
using chopify.Models;
using chopify.Services.Interfaces;

namespace chopify.Services.Implementations
{
    public class SongService(ISongRepository songRepository, ISuggestionRepository suggestionRepository, ICooldownRepository cooldownRepository, IMapper mapper) : ISongService
    {
        private readonly ISongRepository _songRepository = songRepository;
        private readonly ISuggestionRepository _suggestionRepository = suggestionRepository;
        private readonly ICooldownRepository _cooldownRepository = cooldownRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<SongReadDTO>> FetchAsync(string search)
        {
            var result = _mapper.Map<IEnumerable<SongReadDTO>>(await SpotifyService.Instance.FetchTracksAsync(search));

            if (result == null || !result.Any())
                return [];

            var suggestedMarked = await MarkSuggesteds(result);
            var inCooldownMarked = await MarkInCooldowns(suggestedMarked);

            return inCooldownMarked;
        }

        public async Task<IEnumerable<SongReadDTO>> GetMostPopularSongsArgentinaAsync()
        {
            var mostPopularSongs = _mapper.Map<IEnumerable<SongReadDTO>>(await SpotifyService.Instance.GetMostPopularTracksArgentinaAsync());

            if (mostPopularSongs == null || !mostPopularSongs.Any())
                return [];

            var suggestedMarked = await MarkSuggesteds(mostPopularSongs);
            var inCooldownMarked = await MarkInCooldowns(suggestedMarked);

            return inCooldownMarked;
        }

        private async Task<IEnumerable<SongReadDTO>> MarkSuggesteds(IEnumerable<SongReadDTO> songs)
        {
            var suggestions = await _suggestionRepository.GetBySongIdsAsync(songs.Select(song => song.Id));

            if (suggestions == null || !suggestions.Any())
                return songs;

            var suggestionDict = suggestions.ToDictionary(s => s.SpotifySongId);

            foreach (var song in songs)
            {
                if (suggestionDict.TryGetValue(song.Id, out var suggestion))
                {
                    song.IsSuggested = true;
                    song.SuggestedBy = suggestion.SuggestedBy;
                }
            }

            return songs;
        }

        private async Task<IEnumerable<SongReadDTO>> MarkInCooldowns(IEnumerable<SongReadDTO> songs)
        {
            var cooldowns = await _cooldownRepository.GetBySongIdsAsync(songs.Select(song => song.Id));

            if (cooldowns == null || !cooldowns.Any())
                return songs;

            var cooldownDict = cooldowns.ToDictionary(c => c.SpotifySongId);

            foreach (var song in songs)
            {
                if (cooldownDict.TryGetValue(song.Id, out var cooldown))
                {
                    song.IsInCooldown = true;
                    song.CooldownTimeLeft = (cooldown.CooldownEnd - DateTime.UtcNow).TotalSeconds;
                }
            }

            return songs;
        }
    }
}
