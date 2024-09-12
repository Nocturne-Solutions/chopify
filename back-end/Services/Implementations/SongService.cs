using AutoMapper;
using chopify.Data.Repositories.Interfaces;
using chopify.External;
using chopify.Models;
using chopify.Services.Interfaces;

namespace chopify.Services.Implementations
{
    public class SongService(ISongRepository songRepository, ISuggestionRepository suggestionRepository, IMapper mapper) : ISongService
    {
        private readonly ISongRepository _songRepository = songRepository;
        private readonly ISuggestionRepository _suggestionRepository = suggestionRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<SongReadDTO>> FetchAsync(string search)
        {
            var result = _mapper.Map<IEnumerable<SongReadDTO>>(await SpotifyService.Instance.FetchTracksAsync(search));

            if (result == null || !result.Any())
                return [];

            return await MarkSuggesteds(result);
        }

        public async Task<IEnumerable<SongReadDTO>> GetMostPopularSongsArgentinaAsync()
        {
            var mostPopularSongs = _mapper.Map<IEnumerable<SongReadDTO>>(await SpotifyService.Instance.GetMostPopularTracksArgentinaAsync());

            if (mostPopularSongs == null || !mostPopularSongs.Any())
                return [];

            return await MarkSuggesteds(mostPopularSongs);
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
    }
}
