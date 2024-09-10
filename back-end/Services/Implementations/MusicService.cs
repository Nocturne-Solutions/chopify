using AutoMapper;
using chopify.Data.Repositories.Interfaces;
using chopify.External;
using chopify.Models;
using chopify.Services.Interfaces;

namespace chopify.Services.Implementations
{
    public class MusicService(IMusicRepository musicRepository, IMapper mapper) : IMusicService
    {
        private readonly IMusicRepository _musicRepository = musicRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<MusicReadDTO>> FetchAsync(string search)
        {
            return _mapper.Map<IEnumerable<MusicReadDTO>>(await SpotifyService.Instance.FetchTracksAsync(search));
        }

        public async Task<IEnumerable<MusicReadDTO>> GetMostPopularSongsArgentinaAsync()
        {
            return _mapper.Map<IEnumerable<MusicReadDTO>>(await SpotifyService.Instance.GetMostPopularTracksArgentinaAsync());
        }
    }
}
