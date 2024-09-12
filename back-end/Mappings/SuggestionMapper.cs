using AutoMapper;
using chopify.Data.Entities;
using chopify.Models;

namespace chopify.Mappings
{
    public class SuggestionMapper : Profile
    {
        public SuggestionMapper()
        {
            CreateMap<SuggestionUpsertDTO, Suggestion>()
                .ForMember(dest => dest.Votes, opt => opt.MapFrom(src => 1));

            CreateMap<Suggestion, SuggestionReadDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.SpotifySongId));
        }
    }
}
