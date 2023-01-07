using aspnetserver.Data.Models;
using aspnetserver.Data.Models.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace aspnetserver.Data.Repos.Users
{
    public class UsersRepository : IUsersRepository
    {
        private readonly AppDbContext appDbContext;
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;

        public UsersRepository(AppDbContext appDbContext, UserManager<User> userManager, IMapper mapper)
        {
            this.appDbContext = appDbContext;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        public async Task<List<string>> GetAllUsernamesAsync()
        {
            return await appDbContext.Users.Select(u => u.UserName).ToListAsync();
        }

        public async Task<IdentityResult> MapAndCreateUserAsync(UserRegistrationDto userRegistration)
        {
            var user = mapper.Map<User>(userRegistration);
            return await userManager.CreateAsync(user, userRegistration.Password);
        }

        public async Task<List<Claim>> GetClaimsAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.PrimarySid, user.Id),
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
            return await userManager.FindByNameAsync(username);
        }

        public async Task<List<string>> GetUserRolesByUsernameAsync(string username)
        {
            var user = await GetUserByUsernameAsync(username);
            var roles = await userManager.GetRolesAsync(user);

            return roles.ToList();
        }

        public async Task<IdentityResult> AddPostToUserAsync(string username, Post postToAdd)
        {
            var user = await GetUserByUsernameAsync(username);
            user.Posts.Add(postToAdd);

            return await userManager.UpdateAsync(user);
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            return await userManager.FindByIdAsync(id);
        }

        public async Task<bool> CheckPasswordForUserAsync(User user, string password)
        {
            return await userManager.CheckPasswordAsync(user, password);
        }

        public async Task<bool> CheckIsEmailConfirmedForUserAsync(User user)
        {
            return await userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<string> GenerateEmailConfirmationTokenForUserAsync(User user)
        {
            return await userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<IdentityResult> ConfirmEmailForUserAsync(User user, string token)
        {
            return await userManager.ConfirmEmailAsync(user, token);
        }
    }
}
