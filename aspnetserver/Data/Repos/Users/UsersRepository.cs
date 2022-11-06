using aspnetserver.Data.Models;
using aspnetserver.Data.Models.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace aspnetserver.Data.Repos.Users
{
    public class UsersRepository : IUsersRepository
    {
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;

        public UsersRepository(UserManager<User> userManager, IMapper mapper)
        {
            this.userManager = userManager;
            this.mapper = mapper;
        }

        public async Task<IdentityResult> RegisterUserAsync(UserRegistrationDto userRegistration)
        {
            var user = mapper.Map<User>(userRegistration);
            var result = await userManager.CreateAsync(user, userRegistration.Password);
            return result;
        }
    }
}
