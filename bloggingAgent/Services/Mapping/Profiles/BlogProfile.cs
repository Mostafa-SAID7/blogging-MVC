using AutoMapper;
using BloggingAgent.Models.Domain;
using BloggingAgent.Models.DTOs;

namespace BloggingAgent.Services.Mapping.Profiles
{
    public class BlogProfile : Profile
    {
        public BlogProfile()
        {
            CreateMap<BlogPost, BlogPostDto>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags ?? new System.Collections.Generic.List<string>()));

            CreateMap<BlogPostDto, BlogPost>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags ?? new System.Collections.Generic.List<string>()))
                .ForMember(dest => dest.SeoMetadata, opt => opt.MapFrom(src => new SeoMetadata()))
                .ForMember(dest => dest.Analytics, opt => opt.MapFrom(src => new ContentAnalytics()));
        }
    }
}
