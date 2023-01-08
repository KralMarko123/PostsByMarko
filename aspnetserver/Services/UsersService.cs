using aspnetserver.Builders;
using aspnetserver.Data.Models;
using aspnetserver.Data.Models.Dtos;
using aspnetserver.Data.Models.Responses;
using aspnetserver.Data.Repos.Users;
using aspnetserver.Helper;

namespace aspnetserver.Services
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
            var result = await usersRepository.MapAndCreateUserAsync(userRegistration);

            if (result.Succeeded) return new RequestResultBuilder().Created().WithMessage("Successfully Registered").Build();
            else return new RequestResultBuilder().BadRequest().WithMessage(result.Errors.Select(e => e.Description).ToList().First()).Build();
        }

        public async Task<RequestResult> ValidateUserAsync(UserLoginDto userLogin)
        {
            var user = await usersRepository.GetUserByUsernameAsync(userLogin.UserName);

            if (user == null) return new RequestResultBuilder().BadRequest().WithMessage("No account found, please check your credentials and try again").Build();
            if (!await usersRepository.CheckPasswordForUserAsync(user, userLogin.Password)) return new RequestResultBuilder().BadRequest().WithMessage("Invalid password for the given account").Build();
            if (!await usersRepository.CheckIsEmailConfirmedForUserAsync(user)) return new RequestResultBuilder().Forbidden().WithMessage("Please check your email and confirm your account before logging in").Build();


            return new RequestResultBuilder().Ok().WithMessage("Successfully Logged In").WithPayload(new LoginResponse
            {
                Token = await jwtHelper.CreateTokenAsync(user),
                Username = user.UserName,
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = await GetUserRolesByUsernameAsync(user.UserName),
            }).Build();
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await usersRepository.GetUserByUsernameAsync(username);
        }

        public async Task<List<string>> GetUserRolesByUsernameAsync(string username)
        {
            return await usersRepository.GetUserRolesByUsernameAsync(username);
        }

        public async Task<RequestResult> GetAllUsernamesAsync()
        {
            var allUsernames = await usersRepository.GetAllUsernamesAsync();
            return new RequestResultBuilder().Ok().WithPayload(allUsernames).Build();
        }

        public async Task<string> GenerateEmailConfirmationTokenForUserAsync(User user)
        {
            return await usersRepository.GenerateEmailConfirmationTokenForUserAsync(user);
        }
        public async Task<bool> ConfirmEmailForUserAsync(User user, string token)
        {
            var confirmedEmail = await usersRepository.ConfirmEmailForUserAsync(user, token);
            return confirmedEmail.Succeeded;
        }
    }
}
