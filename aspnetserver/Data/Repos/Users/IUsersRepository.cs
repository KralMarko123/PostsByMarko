using aspnetserver.Data.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace aspnetserver.Data.Repos.Users
{
    public interface IUsersRepository
    {
        Task<object> GetUserDetailsForUsernameAsync(string userName);
        Task<IdentityResult> RegisterUserAsync(UserRegistrationDto userRegistration);
        Task<bool> ValidateUserAsync(UserLoginDto userLogin);
        Task<List<Claim>> GetClaimsAsync();
    }
}
