using AutoMapper;
using chopify.Data.Entities;
using chopify.Models;

namespace chopify.Mappings
{
    public class MusicMapper : Profile
    {
        public MusicMapper()
        {
            CreateMap<Music, MusicReadDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.SpotifyId));
        }
    }
}
