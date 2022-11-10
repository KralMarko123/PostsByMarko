using aspnetserver.Data.Models;
using aspnetserver.Data.Models.Dtos;
using Microsoft.AspNetCore.Identity;

namespace aspnetserver.Data.Repos.Users
{
    public interface IUsersRepository
    {
        Task<User> GetUserByUsernameAsync(string userName);
        Task<IdentityResult> RegisterUserAsync(UserRegistrationDto userRegistration);
        Task<bool> ValidateUserAsync(UserLoginDto userLogin);
        Task<string> CreateTokenAsync();
    }
}
