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
        Task<bool> AddPostIdToUserAsync(string username, string postIdToAdd);
        Task<bool> RemovePostIdFromUserAsync(string username, string postIdToRemove);
        Task<List<string>> GetAllUsersAsync();
        Task<string> GenerateEmailConfirmationTokenForUserAsync(User user);
        Task<bool> CheckPasswordForUserAsync(User user, string password);
        Task<bool> CheckIsEmailConfirmedForUserAsync(User user);
        Task<bool> ConfirmEmailForUserAsync(User user, string token);
    }
}
