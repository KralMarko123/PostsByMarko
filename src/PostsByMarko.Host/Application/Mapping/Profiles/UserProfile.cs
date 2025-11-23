using AutoMapper;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Data.Entities;

namespace PostsByMarko.Host.Application.Mapping.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>();
        }
    }
}
