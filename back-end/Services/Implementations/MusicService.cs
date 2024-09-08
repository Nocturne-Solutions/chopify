using AutoMapper;
using chopify.Data.Repositories.Interfaces;
using chopify.External;
using chopify.Models;
using chopify.Services.Interfaces;
using MongoDB.Bson;
using System.Collections.Generic;

namespace chopify.Services.Implementations
{
    public class MusicService(IMusicRepository musicRepository, IMapper mapper) : IMusicService
    {
        private readonly IMusicRepository _musicRepository = musicRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<MusicReadDTO>> FetchAsync(string search)
        {
            return _mapper.Map<IEnumerable<MusicReadDTO>>(await SpotifyService.Instance.FetchTracksAsync(search));
            /*
            var mongoMusic = await _musicRepository.FetchAsync(search);

            if (mongoMusic.Count() < 25)
            {
                var externMusic = await MusicBrainzService.Instance.FetchTracksAsync(search, 25 - mongoMusic.Count());

                var existingIds = new HashSet<ObjectId>(mongoMusic.Select(m => m.Id));

                var uniqueExternMusic = externMusic.Where(track => !existingIds.Contains(track.Id));

                var allMusic = mongoMusic.Concat(uniqueExternMusic);

                _ = Task.Run(async () =>
                {
                    foreach (var song in uniqueExternMusic)
                        await _musicRepository.CreateAsync(song);
                });

                return _mapper.Map<IEnumerable<MusicReadDTO>>(allMusic);
            }

            return _mapper.Map<IEnumerable<MusicReadDTO>>(mongoMusic);*/
        }
    }
}
