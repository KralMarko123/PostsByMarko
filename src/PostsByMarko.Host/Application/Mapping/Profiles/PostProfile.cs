using AutoMapper;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Data.Entities;

namespace PostsByMarko.Host.Application.Mapping.Profiles
{
    public class PostProfile : Profile
    {
        public PostProfile() 
        {
            CreateMap<Post, PostDto>().ReverseMap();
        }
    }
}
