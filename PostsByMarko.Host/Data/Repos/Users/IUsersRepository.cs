using Microsoft.AspNetCore.Identity;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Host.Data.Models.Dtos;
using System.Security.Claims;

namespace PostsByMarko.Host.Data.Repos.Users
{
    public interface IUsersRepository
    {
        Task<IdentityResult> MapAndCreateUserAsync(UserRegistrationDto userRegistration);
        Task<List<Claim>> GetClaimsAsync(User user);
        Task<User> GetUserByUsernameAsync(string username);
        Task<List<string>> GetUserRolesByUsernameAsync(string username);
        Task<User> GetUserByIdAsync(string id);
        Task<bool> AddPostToUserAsync(string username, Post postToAdd);
        Task<List<string>> GetAllUsernamesAsync();
        Task<string> GenerateEmailConfirmationTokenForUserAsync(User user);
        Task<bool> CheckPasswordForUserAsync(User user, string password);
        Task<bool> CheckIsEmailConfirmedForUserAsync(User user);
        Task<IdentityResult> ConfirmEmailForUserAsync(User user, string token);
    }
}
