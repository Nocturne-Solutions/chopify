using AutoMapper;
using chopify.Data.Entities;
using chopify.Models;

namespace chopify.Mappings
{
    public class VoteMapper : Profile
    {
        public VoteMapper()
        {
            CreateMap<Vote, VoteReadDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.SpotifySongId));

            CreateMap<VoteUpsertDTO, Vote>()
                .ForMember(dest => dest.SpotifySongId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Id, opt => opt.Ignore());
                
        }
    }
}
