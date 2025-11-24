using Microsoft.AspNetCore.Identity;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Requests;
using PostsByMarko.Host.Application.Responses;
using PostsByMarko.Host.Data.Entities;

namespace PostsByMarko.Host.Application.Interfaces
{
    public interface IUserService
    {
        Task<User> GetCurrentUserAsync();
        Task CreateUserAsync(RegistrationDto userRegistration);
        Task<UserDto> GetUserByIdAsync(Guid id);
        Task<User> GetUserByEmailAsync(string email);
        Task<IdentityResult> ConfirmEmailForUserAsync(User user, string token);
        Task<LoginResponse> ValidateUserAsync(LoginDto userLogin);
        Task<List<UserDto>> GetUsersAsync(Guid? exceptId, CancellationToken cancellationToken);
        Task<List<string>> GetRolesForEmailAsync(string email);
        Task<string> GenerateEmailConfirmationTokenForUserAsync(User user);
        Task<List<string>> GetRolesForUserAsync(User user);
        Task DeleteUserAsync(Guid id, CancellationToken cancellationToken);
        Task<List<string>> UpdateUserRolesAsync(UpdateUserRolesRequest request, CancellationToken cancellationToken);
        Task<List<AdminDashboardResponse>> GetAdminDashboardAsync(CancellationToken cancellationToken);
    }
}
