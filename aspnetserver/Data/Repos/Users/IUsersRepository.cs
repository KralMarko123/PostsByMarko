using aspnetserver.Data.Models.Dtos;
using Microsoft.AspNetCore.Identity;

namespace aspnetserver.Data.Repos.Users
{
    public interface IUsersRepository
    {
        Task<IdentityResult> RegisterUserAsync(UserRegistrationDto userRegistration);
    }
}
