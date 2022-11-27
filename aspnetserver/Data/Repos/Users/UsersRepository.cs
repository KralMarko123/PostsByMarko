using aspnetserver.Data.Models;
using aspnetserver.Data.Models.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace aspnetserver.Data.Repos.Users
{
    public class UsersRepository : IUsersRepository
    {
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;
        private User? user;

        public UsersRepository(UserManager<User> userManager, IMapper mapper)
        {
            this.userManager = userManager;
            this.mapper = mapper;
        }

        public async Task<object> GetUserDetailsForUsernameAsync(string userName)
        {
            user = await userManager.FindByNameAsync(userName);
            var userRoles = await userManager.GetRolesAsync(user);

            return new { user.UserName, user.Email, user.FirstName, user.LastName, userRoles };
        }

        public async Task<IdentityResult> RegisterUserAsync(UserRegistrationDto userRegistration)
        {
            var user = mapper.Map<User>(userRegistration);
            var result = await userManager.CreateAsync(user, userRegistration.Password);
           
            return result;
        }

        public async Task<bool> ValidateUserAsync(UserLoginDto userLogin)
        {
            user = await userManager.FindByNameAsync(userLogin.UserName);
            var result = user != null && await userManager.CheckPasswordAsync(user, userLogin.Password);

            return result;
        }

        public async Task<List<Claim>> GetClaimsAsync()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }
    }
}
