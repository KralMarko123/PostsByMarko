using PostsByMarko.Host.Data.Models;
using PostsByMarko.Host.Data.Models.Dtos;

namespace PostsByMarko.Host.Services
{
    public interface IUsersService
    {
        Task<RequestResult> MapAndCreateUserAsync(UserRegistrationDto userRegistration);
        Task<RequestResult> ValidateUserAsync(UserLoginDto userLogin);
        Task<RequestResult> GetAllUsersAsync();
        Task<User> GetUserByEmailAsync(string username);
        Task<List<string>> GetRolesForEmailAsync(string username);
        Task<string> GenerateEmailConfirmationTokenForUserAsync(User user);
        Task<bool> ConfirmEmailForUserAsync(User user, string token);
    }
}
