using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PostsByMarko.Host.Data.Models;
using System.Security.Claims;

namespace PostsByMarko.Host.Data.Repos.Users
{
    public class UsersRepository : IUsersRepository
    {
        private readonly AppDbContext appDbContext;
        private readonly UserManager<User> userManager;

        public UsersRepository(AppDbContext appDbContext, UserManager<User> userManager)
        {
            this.appDbContext = appDbContext;
            this.userManager = userManager;
        }

        public async Task<List<string>> GetAllUsernamesAsync()
        {
            return await appDbContext.Users.Select(u => u.UserName).ToListAsync();
        }

        public async Task<bool> MapAndCreateUserAsync(User userToCreate, string passwordForUser)
        {
            var result = await userManager.CreateAsync(userToCreate, passwordForUser);

            return result.Succeeded;
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

        public async Task<bool> AddPostToUserAsync(string username, Post postToAdd)
        {
            var user = await GetUserByUsernameAsync(username);

            user.Posts.Add(postToAdd);

            var result = await userManager.UpdateAsync(user);

            return result.Succeeded;
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

        public async Task<bool> ConfirmEmailForUserAsync(User user, string token)
        {
            var result = await userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded;
        }
    }
}
