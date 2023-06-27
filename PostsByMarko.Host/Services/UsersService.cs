using AutoMapper;
using PostsByMarko.Host.Builders;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Host.Data.Models.Dtos;
using PostsByMarko.Host.Data.Models.Responses;
using PostsByMarko.Host.Data.Repos.Users;
using PostsByMarko.Host.Helper;
using Serilog;

namespace PostsByMarko.Host.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository usersRepository;
        private readonly IJwtHelper jwtHelper;
        private readonly IMapper mapper;

        public UsersService(IUsersRepository usersRepository, IJwtHelper jwtHelper, IMapper mapper)
        {
            this.usersRepository = usersRepository;
            this.jwtHelper = jwtHelper;
            this.mapper = mapper;
        }

        public async Task<RequestResult> MapAndCreateUserAsync(UserRegistrationDto userRegistration)
        {
            var userToCreate = mapper.Map<User>(userRegistration);
            var result = await usersRepository.MapAndCreateUserAsync(userToCreate, userRegistration.Password!);

            if (result) return new RequestResultBuilder().Created().WithMessage("Successfully Registered").Build();
            else return new RequestResultBuilder().BadRequest().WithMessage("Error during user registration").Build();
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
            return await usersRepository.ConfirmEmailForUserAsync(user, token);
        }
    }
}
