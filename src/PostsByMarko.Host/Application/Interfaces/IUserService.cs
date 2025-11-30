using Microsoft.AspNetCore.Identity;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Responses;
using PostsByMarko.Host.Data.Entities;

namespace PostsByMarko.Host.Application.Interfaces
{
    public interface IUserService
    {
        Task<User> GetCurrentUserAsync();
        Task CreateUserAsync(RegistrationDto userRegistration);
        Task<UserDto> GetUserByIdAsync(Guid Id, CancellationToken cancellationToken = default);
        Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<IdentityResult> ConfirmEmailForUserAsync(User user, string token);
        Task<LoginResponse> ValidateUserAsync(LoginDto userLogin, CancellationToken cancellationToken = default);
        Task<List<UserDto>> GetUsersAsync(Guid? exceptId = null, CancellationToken cancellationToken = default);
        Task<string> GenerateEmailConfirmationTokenForUserAsync(User user);
        Task<bool> ValidateUserWithTokenExistsAsync(CancellationToken cancellationToken = default);
    }
}
