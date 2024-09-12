using AutoMapper;
using chopify.Data.Entities;
using chopify.Models;

namespace chopify.Mappings
{
    public class UserMapper : Profile
    {
        public UserMapper() 
        {
            CreateMap<UserUpsertDTO, User>();

            CreateMap<User, UserReadDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));
        }
    }
}
