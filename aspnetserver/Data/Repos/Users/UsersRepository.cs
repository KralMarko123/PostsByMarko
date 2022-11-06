using aspnetserver.Data.Models;
using aspnetserver.Data.Models.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace aspnetserver.Data.Repos.Users
{
    internal static class UsersRepository
    {
        private static readonly UserManager<User> userManager;
        private static readonly IMapper mapper;

        public static async Task<IdentityResult> RegisterUserAsync(UserRegistrationDto userRegistration)
        {
            var user = mapper.Map<User>(userRegistration);
            var result = await userManager.CreateAsync(user, userRegistration.Password);
            return result;
        }
    }
}
