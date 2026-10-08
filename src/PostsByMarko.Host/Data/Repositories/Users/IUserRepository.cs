using Microsoft.AspNetCore.Identity;
using PostsByMarko.Host.Data.Entities;
using System.Security.Claims;

using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Requests;
using PostsByMarko.Host.Application.Responses;

namespace PostsByMarko.Host.Data.Repositories.Users
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(Guid Id, CancellationToken cancellationToken = default);
        Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<PagedResult<UserDto>> GetUsersAsync(PageRequest page, Guid? exceptId = null, CancellationToken cancellationToken = default);
        Task<PagedResult<AdminDashboardResponse>> GetAdminDashboardAsync(Guid exceptId, PageRequest page, CancellationToken cancellationToken = default);
        Task<IdentityResult> CreateUserWithConfirmationEmailAsync(User userToCreate, string passwordForUser, CancellationToken cancellationToken = default);
        Task QueueConfirmationEmailAsync(User user, CancellationToken cancellationToken = default);
        Task<IdentityResult> ConfirmEmailForUserAsync(User user, string token);
        Task<IdentityResult> DeleteUserAsync(User user);
        Task<List<Claim>> GetClaimsAsync(User user);
        Task<string> GenerateEmailConfirmationTokenForUserAsync(User user);
        Task<bool> CheckPasswordForUserAsync(User user, string password);
        Task<bool> CheckIsEmailConfirmedForUserAsync(User user);
        Task<IList<string>> GetRolesForUserAsync(User user);
        Task<IdentityResult> AddRoleToUserAsync(User user, string role);
        Task<IdentityResult> RemoveRoleFromUserAsync(User user, string role);
        Task<IdentityResult> RemoveRoleFromUserUnlessLastMemberAsync(User user, string role, CancellationToken cancellationToken = default);
        Task<IdentityResult> DeleteUserUnlessLastMemberInRoleAsync(User user, string role, CancellationToken cancellationToken = default);
        Task<IdentityResult> UpdateUserAsync(User user);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
