using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PostsByMarko.Host.Data;
using PostsByMarko.Host.Data.Models;
using System.Security.Claims;

namespace PostsByMarko.Host.Repos.Users
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
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var roles = await userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await userManager.FindByEmailAsync(email);
        }

        public async Task<List<string>> GetRolesForEmailAsync(string email)
        {
            var roles = new List<string>();
            var user = await GetUserByEmailAsync(email);

            if (user != null)
            {
                roles.AddRange(await userManager.GetRolesAsync(user));
            }

            return roles;
        }

        public async Task<bool> AddPostToUserAsync(User user, Post post)
        {
            user.Posts.Add(post);

            var result = await userManager.UpdateAsync(user);

            return result.Succeeded;
        }

        public async Task<bool> RemovePostFromUserAsync(string email, Post post)
        {
            var user = await GetUserByEmailAsync(email);

            user.Posts!.Remove(post);

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

        public async Task<List<string>> GetRolesForUserAsync(User user)
        {
            var roles = new List<string>();

            if(user != null)
            {
                roles.AddRange(await userManager.GetRolesAsync(user));
            }

            return roles;
        }

        public async Task<bool> AddRoleToUserAsync(User user, string role)
        {
            var result = await userManager.AddToRoleAsync(user, role);

            return result.Succeeded;
        }

        public Task<List<User>> GetAllUsersAsync()
        {
            var result = appDbContext.Users.Include(u => u.Posts).ToListAsync();

            return result;
        }

        public async Task<bool> DeleteUserAsync(User user)
        {
            var result = await userManager.DeleteAsync(user);
            
            return result.Succeeded;
        }

        public async Task<bool> RemoveRoleFromUserAsync(User user, string role)
        {
            var result = await userManager.RemoveFromRoleAsync(user, role);

            return result.Succeeded;
        }
    }
}
