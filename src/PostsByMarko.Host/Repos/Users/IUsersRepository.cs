using PostsByMarko.Host.Data.Models;
using System.Security.Claims;

namespace PostsByMarko.Host.Data.Repos.Users
{
    public interface IUsersRepository
    {
        Task<bool> MapAndCreateUserAsync(User userToCreate, string passwordForUser);
        Task<List<Claim>> GetClaimsAsync(User user);
        Task<User> GetUserByEmailAsync(string email);
        Task<List<string>> GetRolesForEmailAsync(string email);
        Task<User> GetUserByIdAsync(string id);
        Task<bool> AddPostToUserAsync(string username, Post post);
        Task<bool> RemovePostFromUserAsync(string username, Post post);
        Task<List<string>> GetAllUsersAsync();
        Task<string> GenerateEmailConfirmationTokenForUserAsync(User user);
        Task<bool> CheckPasswordForUserAsync(User user, string password);
        Task<bool> CheckIsEmailConfirmedForUserAsync(User user);
        Task<bool> ConfirmEmailForUserAsync(User user, string token);
        Task<List<string>> GetRolesForUserAsync(User user);
        Task<bool> AddRolesToUserAsync(User user, IEnumerable<string> roles);
        Task<bool> RemoveRolesFromUserAsync(User user, IEnumerable<string> roles);
    }
}
