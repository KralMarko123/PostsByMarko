using System.Linq;
using NuGet.Versioning;
using PostsByMarko.Host.Builders;
using PostsByMarko.Host.Constants;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Host.Data.Models.Dtos;
using PostsByMarko.Host.Data.Models.Responses;
using PostsByMarko.Host.Data.Repos.Posts;
using PostsByMarko.Host.Data.Repos.Users;
using PostsByMarko.Host.Helper;

namespace PostsByMarko.Host.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository usersRepository;
        private readonly IJwtHelper jwtHelper;

        public UsersService(IUsersRepository usersRepository, IJwtHelper jwtHelper)
        {
            this.usersRepository = usersRepository;
            this.jwtHelper = jwtHelper;
        }

        public async Task<RequestResult> MapAndCreateUserAsync(UserRegistrationDto userRegistration)
        {
            var badRequest = new RequestResultBuilder().BadRequest().WithMessage("Error during user registration").Build();
            var successfulRequest = new RequestResultBuilder().Created().WithMessage("Successfully Registered").Build();
            var newUser = new User(userRegistration.Email!);
            var result = await usersRepository.MapAndCreateUserAsync(newUser, userRegistration.Password!);

            if (result) return successfulRequest;
            else return badRequest;
        }

        public async Task<RequestResult> ValidateUserAsync(UserLoginDto userLogin)
        {
            var user = await usersRepository.GetUserByEmailAsync(userLogin.Email!);

            if (user == null) return new RequestResultBuilder().BadRequest().WithMessage("No account found, please check your credentials and try again").Build();
            if (!await usersRepository.CheckPasswordForUserAsync(user, userLogin.Password!)) return new RequestResultBuilder().BadRequest().WithMessage("Invalid password for the given account").Build();
            if (!await usersRepository.CheckIsEmailConfirmedForUserAsync(user)) return new RequestResultBuilder().Forbidden().WithMessage("Please check your email and confirm your account before logging in").Build();

            return new RequestResultBuilder().Ok().WithMessage("Successfully Logged In").WithPayload(new LoginResponse
            {
                Token = await jwtHelper.CreateTokenAsync(user),
                Email = user.Email,
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = await usersRepository.GetRolesForEmailAsync(user.Email),
            }).Build();
        }

        public async Task<User> GetUserByEmailAsync(string username)
        {
            return await usersRepository.GetUserByEmailAsync(username);
        }

        public async Task<RequestResult> GetRolesForEmailAsync(string email)
        {
            var roles = await usersRepository.GetRolesForEmailAsync(email);
            var result = new RequestResultBuilder().Ok().WithPayload(roles).WithMessage(roles.Count > 0 ? "" : "Email has no roles associated with it").Build();

            return result;
        }

        public async Task<RequestResult> GetAllUsersAsync()
        {
            var allUsernames = await usersRepository.GetAllUsersAsync();

            return new RequestResultBuilder().Ok().WithPayload(allUsernames).Build();
        }

        public async Task<string> GenerateEmailConfirmationTokenForUserAsync(User user)
        {
            return await usersRepository.GenerateEmailConfirmationTokenForUserAsync(user);
        }

        public async Task<bool> ConfirmEmailForUserAsync(User user, string token)
        {
            return await usersRepository.ConfirmEmailForUserAsync(user, token);
        }

        public async Task<RequestResult> GetUserByIdAsync(string id)
        {
            var notfoundRequest = new RequestResultBuilder().NotFound().WithMessage($"User with Id: {id} was not found").Build();
            var user = await usersRepository.GetUserByIdAsync(id);

            if (user == null)
                return notfoundRequest;

            return new RequestResultBuilder()
                .Ok()
                .WithPayload(user)
                .Build();
        }

        public async Task<List<string>> GetRolesForUserAsync(User user)
        {
            return await usersRepository.GetRolesForUserAsync(user);
        }

        public async Task<RequestResult> AddRolesToUserAsync(User user, IEnumerable<string> roles)
        {
            var badRequest = new RequestResultBuilder().BadRequest().WithMessage("Error during role addition").Build();
            var rolesAdded = await usersRepository.AddRolesToUserAsync(user, roles);

            if (rolesAdded)
            {
                return new RequestResultBuilder().Ok().WithMessage("Roles successfully added to user").Build();
            }
            else return badRequest;
        }

        public async Task<RequestResult> RemoveRolesFromUserAsync(User user, IEnumerable<string> roles)
        {
            var badRequest = new RequestResultBuilder().BadRequest().WithMessage("Error during role removal").Build();
            var rolesRemoved = await usersRepository.RemoveRolesFromUserAsync(user, roles);

            if (rolesRemoved)
            {
                return new RequestResultBuilder().Ok().WithMessage("Roles successfully removed from user").Build();
            }
            else return badRequest;
        }

        public async Task<RequestResult> GetAdminDashboard(User admin)
        {
            var noDataRequest = new RequestResultBuilder().NoContent().WithMessage("No data available").Build();
            var users = await usersRepository.GetAllUsersAsync();

            users = users.FindAll(u => u.Id != admin.Id);

            if (users == null || users.Count == 0)
                return noDataRequest;


            var result = new List<AdminDashboardResponse>();

            foreach (var user in users)
            {
                var roles = await usersRepository.GetRolesForEmailAsync(user.Email);
                result.Add(new AdminDashboardResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    NumberOfPosts = user.Posts.Count,
                    LastPostedAt = user.Posts.MaxBy(p => p.LastUpdatedDate)?.LastUpdatedDate,
                    Roles = roles
                });

            }

            return new RequestResultBuilder()
                .Ok()
                .WithPayload(result)
                .Build();
        }
    }
}
