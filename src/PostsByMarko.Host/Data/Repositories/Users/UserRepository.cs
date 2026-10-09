using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PostsByMarko.Host.Application.Constants;
using PostsByMarko.Host.Data.Entities;
using System.Data;
using System.Security.Claims;

using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Requests;
using PostsByMarko.Host.Application.Responses;

namespace PostsByMarko.Host.Data.Repositories.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext appDbContext;
        private readonly UserManager<User> userManager;

        public UserRepository(AppDbContext appDbContext, UserManager<User> userManager)
        {
            this.appDbContext = appDbContext;
            this.userManager = userManager;
        }

        public async Task<IdentityResult> CreateUserWithConfirmationEmailAsync(
            User userToCreate,
            string passwordForUser,
            CancellationToken cancellationToken = default)
        {
            await using var transaction = await appDbContext.Database.BeginTransactionAsync(cancellationToken);
            var result = await userManager.CreateAsync(userToCreate, passwordForUser);

            if (!result.Succeeded)
            {
                return result;
            }

            appDbContext.EmailOutboxMessages.Add(CreateConfirmationMessage(userToCreate));
            await appDbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return result;
        }

        public async Task QueueConfirmationEmailAsync(User user, CancellationToken cancellationToken = default)
        {
            await using var transaction = await appDbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable, cancellationToken);

            await appDbContext.Users
                .FromSqlInterpolated($"SELECT * FROM AspNetUsers WHERE Id = {user.Id} FOR UPDATE")
                .ToListAsync(cancellationToken);

            var message = await appDbContext.EmailOutboxMessages
                .SingleOrDefaultAsync(item => item.UserId == user.Id, cancellationToken);
            var now = DateTime.UtcNow;

            if (message is null)
            {
                appDbContext.EmailOutboxMessages.Add(CreateConfirmationMessage(user));
            }
            else if (message.SentAt.HasValue && message.SentAt.Value <= now.AddMinutes(-15))
            {
                message.RecipientEmail = user.Email!;
                message.AvailableAt = now;
                message.SentAt = null;
                message.AttemptCount = 0;
                message.LockId = null;
                message.LockedUntil = null;
                message.LastError = null;
            }

            await appDbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        private static EmailOutboxMessage CreateConfirmationMessage(User user) => new()
        {
            UserId = user.Id,
            RecipientEmail = user.Email!
        };

        public async Task<List<Claim>> GetClaimsAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.PrimarySid, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var roles = await userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await userManager.Users
                .SingleOrDefaultAsync(u => u.Email == email, cancellationToken);

            return user;
        }

        public async Task<User?> GetUserByIdAsync(Guid Id, CancellationToken cancellationToken = default)
        {
            var user = await userManager.Users
                .SingleOrDefaultAsync(u => u.Id == Id, cancellationToken);

            return user;
        }

        public async Task<bool> CheckPasswordForUserAsync(User user, string password)
        {
            if (await userManager.IsLockedOutAsync(user))
                return false;

            if (!await userManager.CheckPasswordAsync(user, password))
            {
                await userManager.AccessFailedAsync(user);
                return false;
            }

            return (await userManager.ResetAccessFailedCountAsync(user)).Succeeded;
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
            return await ExecuteRoleChangeAsync(user, () => userManager.AddToRoleAsync(user, role));
        }

        public Task<PagedResult<UserDto>> GetUsersAsync(PageRequest page, Guid? exceptId = null, CancellationToken cancellationToken = default)
        {
            return appDbContext.Users.AsNoTracking()
                .Where(user => !exceptId.HasValue || user.Id != exceptId.Value)
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName!,
                    LastName = user.LastName!
                })
                .OrderBy(user => user.Email).ThenBy(user => user.Id)
                .ToPageAsync(page, cancellationToken);
        }

        public async Task<PagedResult<AdminDashboardResponse>> GetAdminDashboardAsync(
            Guid exceptId, PageRequest page, CancellationToken cancellationToken = default)
        {
            var result = await appDbContext.Users.AsNoTracking()
                .Where(user => user.Id != exceptId)
                .Select(user => new AdminDashboardResponse
                {
                    UserId = user.Id,
                    Email = user.Email!,
                    NumberOfPosts = user.Posts.Count(),
                    LastPostedAt = user.Posts.Max(post => (DateTime?)post.LastUpdatedAt)
                })
                .OrderBy(user => user.Email).ThenBy(user => user.UserId)
                .ToPageAsync(page, cancellationToken);

            if (result.Items.Count == 0)
                return result;

            // Fetch roles for this page together, rather than querying Identity once per user.
            var userIds = result.Items.Select(user => user.UserId).ToArray();
            var roles = await (from membership in appDbContext.UserRoles.AsNoTracking()
                               join role in appDbContext.Roles.AsNoTracking() on membership.RoleId equals role.Id
                               where userIds.Contains(membership.UserId)
                               orderby role.Name
                               select new { membership.UserId, role.Name }).ToListAsync(cancellationToken);
            var rolesByUser = roles.ToLookup(role => role.UserId, role => role.Name!);

            foreach (var user in result.Items)
                user.Roles = rolesByUser[user.UserId].ToList();

            return result;
        }

        public async Task<IdentityResult> RemoveRoleFromUserAsync(User user, string role)
        {
            return await ExecuteRoleChangeAsync(user, () => userManager.RemoveFromRoleAsync(user, role));
        }

        private async Task<IdentityResult> ExecuteRoleChangeAsync(User user, Func<Task<IdentityResult>> changeRole)
        {
            await using var transaction = await appDbContext.Database.BeginTransactionAsync();
            var result = await ChangeRoleAndStampAsync(user, changeRole);
            if (result.Succeeded)
            {
                await transaction.CommitAsync();
            }

            return result;
        }

        private async Task<IdentityResult> ChangeRoleAndStampAsync(User user, Func<Task<IdentityResult>> changeRole)
        {
            var result = await changeRole();
            return result.Succeeded ? await userManager.UpdateSecurityStampAsync(user) : result;
        }

        public Task<IdentityResult> RemoveRoleFromUserUnlessLastMemberAsync(
            User user, string role, CancellationToken cancellationToken = default) =>
            ExecuteUnlessLastMemberInRoleAsync(
                user,
                role,
                () => ChangeRoleAndStampAsync(user, () => userManager.RemoveFromRoleAsync(user, role)),
                cancellationToken);

        public Task<IdentityResult> DeleteUserUnlessLastMemberInRoleAsync(
            User user, string role, CancellationToken cancellationToken = default) =>
            ExecuteUnlessLastMemberInRoleAsync(
                user,
                role,
                () => userManager.DeleteAsync(user),
                cancellationToken);

        private async Task<IdentityResult> ExecuteUnlessLastMemberInRoleAsync(
            User user,
            string role,
            Func<Task<IdentityResult>> operation,
            CancellationToken cancellationToken)
        {
            await using var transaction = await appDbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable, cancellationToken);

            var normalizedRole = role.ToUpperInvariant();
            var lockedRoles = await appDbContext.Roles
                .FromSqlInterpolated($"SELECT * FROM AspNetRoles WHERE NormalizedName = {normalizedRole} FOR UPDATE")
                .ToListAsync(cancellationToken);
            var lockedRole = lockedRoles.SingleOrDefault();

            if (lockedRole is null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "RoleNotFound",
                    Description = $"Role '{role}' does not exist."
                });
            }

            var memberCount = await appDbContext.UserRoles
                .CountAsync(userRole => userRole.RoleId == lockedRole.Id, cancellationToken);
            if (memberCount <= 1)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = IdentityErrorCodes.LastMemberInRole,
                    Description = $"The last member of role '{role}' cannot be removed."
                });
            }

            var result = await operation();
            if (result.Succeeded)
            {
                await transaction.CommitAsync(cancellationToken);
            }

            return result;
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
