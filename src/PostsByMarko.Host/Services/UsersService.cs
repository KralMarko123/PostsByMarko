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
            var newUser = new User(userRegistration.Email!);
            var result = await usersRepository.MapAndCreateUserAsync(newUser, userRegistration.Password!);

            if (result) return new RequestResultBuilder().Created().WithMessage("Successfully Registered").Build();
            else return new RequestResultBuilder().BadRequest().WithMessage("Error during user registration").Build();
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
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = await GetRolesForEmailAsync(user.Email),
            }).Build();
        }

        public async Task<User> GetUserByEmailAsync(string username)
        {
            return await usersRepository.GetUserByEmailAsync(username);
        }

        public async Task<List<string>> GetRolesForEmailAsync(string username)
        {
            return await usersRepository.GetRolesForEmailAsync(username);
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
    }
}
