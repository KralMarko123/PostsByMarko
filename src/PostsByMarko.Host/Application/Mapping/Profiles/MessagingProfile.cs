using AutoMapper;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Data.Entities;

namespace PostsByMarko.Host.Application.Mapping.Profiles
{
    public class MessagingProfile : Profile
    {
        public MessagingProfile() 
        {
            CreateMap<Message, MessageDto>().ReverseMap();

            CreateMap<ChatUser, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName));

            CreateMap<Chat, ChatDto>()
                .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.ChatUsers));
        }
    }
}
