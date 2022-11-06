using aspnetserver.Data.Models;
using aspnetserver.Data.Models.Dtos;
using AutoMapper;

namespace aspnetserver.Data.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<UserRegistrationDto, User>();
        }
    }
}
