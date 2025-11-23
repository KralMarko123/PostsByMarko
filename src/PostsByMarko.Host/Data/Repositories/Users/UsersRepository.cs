using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PostsByMarko.Host.Data.Entities;
using System.Security.Claims;

namespace PostsByMarko.Host.Data.Repositories.Users
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

        public async Task<IdentityResult> MapAndCreateUserAsync(User userToCreate, string passwordForUser)
        {
            return await userManager.CreateAsync(userToCreate, passwordForUser);
        }

        public async Task<List<Claim>> GetClaimsAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.PrimarySid, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var roles = await userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await userManager.FindByEmailAsync(email);
        }

        public async Task<List<string>> GetRolesForUser(User user)
        {
            var result = await userManager.GetRolesAsync(user);

            return [.. result];
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

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await userManager.FindByIdAsync(id.ToString());
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

        public async Task<IList<string>> GetRolesForUserAsync(User user)
        {
            return await userManager.GetRolesAsync(user);
        }

        public async Task<IdentityResult> AddRoleToUserAsync(User user, string role)
        {
            return await userManager.AddToRoleAsync(user, role);
        }

        public Task<List<User>> GetUsersAsync(Guid? exceptId = null, CancellationToken cancellationToken = default)
        {
            var result = appDbContext.Users.ToListAsync(cancellationToken);

            if(exceptId.HasValue)
            {
                result = appDbContext.Users.Where(u => u.Id != exceptId.Value).ToListAsync(cancellationToken);
            }

            return result;
        }

        public async Task<IdentityResult> RemoveRoleFromUserAsync(User user, string role)
        {
            return await userManager.RemoveFromRoleAsync(user, role);
        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> DeleteUserAsync(User user)
        {
            return await userManager.DeleteAsync(user);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await appDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
