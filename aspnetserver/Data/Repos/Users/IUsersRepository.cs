using aspnetserver.Data.Models;
using aspnetserver.Data.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace aspnetserver.Data.Repos.Users
{
    public interface IUsersRepository
    {
        Task<object> GetUserDetailsByUsernameAsync(string username);
        Task<IdentityResult> RegisterUserAsync(UserRegistrationDto userRegistration);
        Task<bool> ValidateUserAsync(UserLoginDto userLogin);
        Task<List<Claim>> GetClaimsAsync();
        Task<User> GetUserByUsernameAsync(string username);
        Task<bool> AddPostToUserAsync(string username, Post postToAdd);
    }
}
