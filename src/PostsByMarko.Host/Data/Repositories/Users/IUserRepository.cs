using Microsoft.AspNetCore.Identity;
using PostsByMarko.Host.Data.Entities;
using System.Security.Claims;

namespace PostsByMarko.Host.Data.Repositories.Users
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(Guid Id, CancellationToken cancellationToken = default);
        Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<List<User>> GetUsersAsync(Guid? exceptId = null, CancellationToken cancellationToken = default);
        Task<IdentityResult> MapAndCreateUserAsync(User userToCreate, string passwordForUser);
        Task<IdentityResult> ConfirmEmailForUserAsync(User user, string token);
        Task<IdentityResult> DeleteUserAsync(User user);
        Task<List<Claim>> GetClaimsAsync(User user);
        Task<string> GenerateEmailConfirmationTokenForUserAsync(User user);
        Task<bool> CheckPasswordForUserAsync(User user, string password);
        Task<bool> CheckIsEmailConfirmedForUserAsync(User user);
        Task<IList<string>> GetRolesForUserAsync(User user);
        Task<IdentityResult> AddRoleToUserAsync(User user, string role);
        Task<IdentityResult> RemoveRoleFromUserAsync(User user, string role);
        Task<IdentityResult> UpdateUserAsync(User user);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
