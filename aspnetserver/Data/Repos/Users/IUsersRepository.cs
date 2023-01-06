using aspnetserver.Data.Models;
using aspnetserver.Data.Models.Dtos;
using aspnetserver.Data.Models.Responses;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace aspnetserver.Data.Repos.Users
{
    public interface IUsersRepository
    {
        Task<UserValidationResponse> MapAndCreateUserAsync(UserRegistrationDto userRegistration);
        Task<UserValidationResponse> ValidateUserAsync(UserLoginDto userLogin);
        Task<List<Claim>> GetClaimsAsync(User user);
        Task<User> GetUserByUsernameAsync(string username);
        Task<List<string>> GetUserRolesByUsernameAsync(string username);
        Task<User> GetUserByIdAsync(string id);
        Task<IdentityResult> AddPostToUserAsync(string username, Post postToAdd);
        Task<List<string>> GetAllUsernamesAsync();
    }
}
