using PostsByMarko.Host.Builders;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Host.Data.Models.Dtos;
using PostsByMarko.Host.Data.Models.Responses;
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

        public async Task<RequestResult> AddRoleToUserAsync(string userId, string role)
        {
            var notfoundRequest = new RequestResultBuilder().NotFound().WithMessage($"User with Id: {userId} was not found").Build();
            var badRequest = new RequestResultBuilder().BadRequest().WithMessage("Error during role addition").Build();

            var user = await usersRepository.GetUserByIdAsync(userId);
            if (user == null)
                return notfoundRequest;

            var roleAdded = await usersRepository.AddRoleToUserAsync(user, role);

            if (!roleAdded)
                return badRequest;
                
            return new RequestResultBuilder()
                .Ok()
                .WithMessage("Role successfully added to user")
                .Build();
        }

        public async Task<RequestResult> RemoveRoleFromUserAsync(string userId, string role)
        {
            var notfoundRequest = new RequestResultBuilder().NotFound().WithMessage($"User with Id: {userId} was not found").Build();
            var badRequest = new RequestResultBuilder().BadRequest().WithMessage("Error during role removal").Build();

            var user = await usersRepository.GetUserByIdAsync(userId);
            if (user == null)
                return notfoundRequest;

            var roleRemoved = await usersRepository.RemoveRoleFromUserAsync(user, role);

            if (!roleRemoved)
                return badRequest;

            return new RequestResultBuilder()
                .Ok()
                .WithMessage("Role successfully removed from user")
                .Build();
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
                    CreatedAt = user.CreatedAt,
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

        public async Task<RequestResult> DeleteUser(string userId)
        {
            var badRequest = new RequestResultBuilder().BadRequest().WithMessage($"Error while removing user with id: {userId}").Build();
            var notFoundRequest = new RequestResultBuilder().NotFound().WithMessage($"User with Id: {userId} was not found").Build();
            var user = await usersRepository.GetUserByIdAsync(userId);

            if (user == null)
                return notFoundRequest;

            var userRemovedSuccessfully = await usersRepository.DeleteUserAsync(user);

            if(!userRemovedSuccessfully)
                return badRequest;

            return new RequestResultBuilder()
                .Ok()
                .WithMessage($"User with Id: {userId} removed successfully")
                .Build();
        }
    }
}
