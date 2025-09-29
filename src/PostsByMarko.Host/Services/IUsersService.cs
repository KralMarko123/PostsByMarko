using PostsByMarko.Host.Data.Models;
using PostsByMarko.Host.Data.Models.Dtos;

namespace PostsByMarko.Host.Services
{
    public interface IUsersService
    {
        Task<RequestResult> MapAndCreateUserAsync(UserRegistrationDto userRegistration);
        Task<RequestResult> ValidateUserAsync(UserLoginDto userLogin);
        Task<RequestResult> GetAllUsersAsync();
        Task<User> GetUserByEmailAsync(string email);
        Task<RequestResult> GetRolesForEmailAsync(string email);
        Task<string> GenerateEmailConfirmationTokenForUserAsync(User user);
        Task<bool> ConfirmEmailForUserAsync(User user, string token);
        Task<RequestResult> GetUserByIdAsync(string id);
        Task<List<string>> GetRolesForUserAsync(User user);
        Task<RequestResult> AddRolesToUserAsync(User user, IEnumerable<string> roles);
        Task<RequestResult> RemoveRolesFromUserAsync(User user, IEnumerable<string> roles);
        Task<RequestResult> GetAdminDashboard(User admin);
    }
}
