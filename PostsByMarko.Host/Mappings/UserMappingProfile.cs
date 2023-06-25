using AutoMapper;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Host.Data.Models.Dtos;

namespace PostsByMarko.Host.Data.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<UserRegistrationDto, User>();
        }
    }
}
