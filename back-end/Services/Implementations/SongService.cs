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
            return _mapper.Map<IEnumerable<SongReadDTO>>(await SpotifyService.Instance.FetchTracksAsync(search));
        }

        public async Task<IEnumerable<SongReadDTO>> GetMostPopularSongsArgentinaAsync()
        {
            var mostPopularSongs = _mapper.Map<IEnumerable<SongReadDTO>>(await SpotifyService.Instance.GetMostPopularTracksArgentinaAsync());

            foreach (var song in mostPopularSongs)
                if (await _suggestionRepository.IsSuggested(song.Id))
                    song.IsSuggested = true;

            return mostPopularSongs;
        }
    }
}
