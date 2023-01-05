using aspnetserver.Data.Models;
using aspnetserver.Data.Models.Dtos;
using aspnetserver.Data.Models.Responses;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace aspnetserver.Data.Repos.Users
{
    public class UsersRepository : IUsersRepository
    {
        private readonly AppDbContext appDbContext;
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;
        private User? user;

        public UsersRepository(AppDbContext appDbContext, UserManager<User> userManager, IMapper mapper)
        {
            this.appDbContext = appDbContext;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        public async Task<List<string>> GetAllUsernamesAsync()
        {
            return await appDbContext.Users.Select(u => u.UserName).ToListAsync();
        }

        public async Task<UserValidationResponse> MapAndCreateUserAsync(UserRegistrationDto userRegistration)
        {
            user = mapper.Map<User>(userRegistration);
            var result = await userManager.CreateAsync(user, userRegistration.Password);

            if (result.Succeeded) return new UserValidationResponse { IsValid = true };
            else return new UserValidationResponse { IsValid = false, Reason = result.Errors.Select(e => e.Code.ToUpper()).ToList().First() };
        }

        public async Task<UserValidationResponse> ValidateUserAsync(UserLoginDto userLogin)
        {
            user = await userManager.FindByNameAsync(userLogin.UserName);

            if (user == null) return new UserValidationResponse
            {
                IsValid = false,
                Reason = "NO ACCOUNT"
            };

            if (!await userManager.CheckPasswordAsync(user, userLogin.Password)) return new UserValidationResponse
            {
                IsValid = false,
                Reason = "INVALID PASSWORD"
            };

            if (!await userManager.IsEmailConfirmedAsync(user)) return new UserValidationResponse
            {
                IsValid = false,
                Reason = "NOT CONFIRMED"
            };


            return new UserValidationResponse { IsValid = true };
        }

        public async Task<List<Claim>> GetClaimsAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.PrimarySid, user.Id),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            user = await userManager.FindByNameAsync(username);
            return user;
        }

        public async Task<List<string>> GetUserRolesByUsername(string username)
        {
            user = await GetUserByUsernameAsync(username);
            var roles = await userManager.GetRolesAsync(user);

            return roles.ToList();
        }

        public async Task<IdentityResult> AddPostToUserAsync(string username, Post postToAdd)
        {
            user = await GetUserByUsernameAsync(username);
            user.Posts.Add(postToAdd);

            return await userManager.UpdateAsync(user);
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            user = await userManager.FindByIdAsync(id);
            return user;
        }
    }
}
