using aspnetserver.Data.Models;
using aspnetserver.Data.Models.Dtos;
using aspnetserver.Data.Models.Responses;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace aspnetserver.Data.Repos.Users
{
    public interface IUsersRepository
    {
        Task<IdentityResult> RegisterUserAsync(UserRegistrationDto userRegistration);
        Task<bool> ValidateUserAsync(UserLoginDto userLogin);
        Task<List<Claim>> GetClaimsAsync();
        Task<User> GetUserByUsernameAsync(string username);
        Task<List<string>> GetUserRolesByUsername(string username);
        Task<ProfileResponse> GetUserProfileByUsername(string username);
        Task<User> GetUserByIdAsync(string id);
        Task<IdentityResult> AddPostToUserAsync(string username, Post postToAdd);
    }
}
