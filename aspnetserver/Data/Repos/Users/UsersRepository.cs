using aspnetserver.Data.Models;
using aspnetserver.Data.Models.Dtos;
using aspnetserver.Data.Models.Responses;
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

        public async Task<IdentityResult> RegisterUserAsync(UserRegistrationDto userRegistration)
        {
            var user = mapper.Map<User>(userRegistration);

            return await userManager.CreateAsync(user, userRegistration.Password);
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

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            user = await userManager.FindByNameAsync(username);
            return user;
        }

        public async Task<List<string>> GetUserRolesByUsername(string username)
        {
            user = await GetUserByUsernameAsync(username);
            var roles = await userManager.GetRolesAsync(user);

            return roles.ToList();
        }

        public async Task<IdentityResult> AddPostToUserAsync(string username, Post postToAdd)
        {
            user = await GetUserByUsernameAsync(username);
            user.Posts.Add(postToAdd);

            return await userManager.UpdateAsync(user);
        }

        public async Task<UserResponse> GetUserProfileByUsername(string username)
        {
            user = await GetUserByUsernameAsync(username);

            return new UserResponse()
            {
                UserId = user.Id,
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = await GetUserRolesByUsername(user.UserName)
            };
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            user = await userManager.FindByIdAsync(id);
            return user;
        }
    }
}
