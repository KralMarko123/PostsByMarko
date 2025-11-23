using AutoMapper;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Data.Entities;

namespace PostsByMarko.Host.Application.Mapping.Profiles
{
    public class RegistrationPofile : Profile
    {
        public RegistrationPofile()
        {
            CreateMap<RegistrationDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
        }
    }
}
