﻿using AutoMapper;
using chopify.Data.Entities;
using chopify.Models;
using SpotifyAPI.Web;

namespace chopify.Mappings
{
    public class MusicMapper : Profile
    {
        public MusicMapper()
        {
            CreateMap<Music, MusicReadDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.SpotifyId));

            CreateMap<FullTrack, MusicReadDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Artist, opt => opt.MapFrom(src => string.Join(", ", src.Artists.Select(ac => ac.Name))))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.FirstReleaseDate, opt => opt.MapFrom(src => src.Album.ReleaseDate))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => new TimeSpan(0, 0, 0, 0, src.DurationMs)))
                .ForMember(dest => dest.CoverUrl, opt => opt.MapFrom(src => src.Album.Images[0].Url));
        }
    }
}
