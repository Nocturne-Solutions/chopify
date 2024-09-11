using AutoMapper;
using chopify.Data.Entities;
using chopify.Models;
using SpotifyAPI.Web;

namespace chopify.Mappings
{
    public class SuggestionMapper : Profile
    {
        public SuggestionMapper()
        {
            CreateMap<SuggestionDTO, Suggestion>()
                .ForMember(dest => dest.Votes, opt => opt.MapFrom(src => 1));
        }
    }
}
