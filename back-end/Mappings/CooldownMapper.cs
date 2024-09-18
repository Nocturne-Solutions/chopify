using AutoMapper;
using chopify.Data.Entities;
using chopify.Models;

namespace chopify.Mappings
{
    public class CooldownMapper : Profile
    {
        public CooldownMapper()
        {
            CreateMap<Cooldown, CooldownReadDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.SpotifySongId))
                .ForMember(dest => dest.CooldownTimeLeft, opt => opt.MapFrom(src => (src.CooldownEnd - DateTime.UtcNow).TotalSeconds));
        }
    }
}
