using AutoMapper;
using chopify.Data.Entities;
using chopify.Models;

namespace chopify.Mappings
{
    public class WinnerMapper : Profile
    {
        public WinnerMapper()
        {
            CreateMap<Suggestion, Winner>()
                .ForMember(w => w.Id, opt => opt.Ignore())
                .ForMember(w => w.CreatedAt, Opt => Opt.MapFrom(s => DateTime.UtcNow))
                .ForMember(w => w.RoundNumber, opt => opt.Ignore());

            CreateMap<SongReadDTO, Winner>()
                .ForMember(w => w.Id, opt => opt.Ignore())
                .ForMember(w => w.SpotifySongId, opt => opt.MapFrom(s => s.Id))
                .ForMember(w => w.CreatedAt, Opt => Opt.MapFrom(s => DateTime.UtcNow));

            CreateMap<Winner, WinnerReadDTO>()
                .ForMember(w => w.Id, opt => opt.MapFrom(w => w.SpotifySongId));
        }
    }
}
