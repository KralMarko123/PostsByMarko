using AutoMapper;
using Microsoft.AspNetCore.Identity;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Enums;
using PostsByMarko.Host.Application.Exceptions;
using PostsByMarko.Host.Application.Helper;
using PostsByMarko.Host.Application.Interfaces;
using PostsByMarko.Host.Application.Requests;
using PostsByMarko.Host.Application.Responses;
using PostsByMarko.Host.Data.Entities;
using PostsByMarko.Host.Data.Repositories.Users;

namespace PostsByMarko.Host.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IEmailService emailService;
        private readonly IJwtHelper jwtHelper;
        private readonly IMapper mapper;
        private readonly ICurrentRequestAccessor currentRequestAccessor;

        public UserService(IUserRepository userRepository, IEmailService emailService, IJwtHelper jwtHelper, IMapper mapper, ICurrentRequestAccessor currentRequestAccessor)
        {
            this.userRepository = userRepository;
            this.emailService = emailService;
            this.jwtHelper = jwtHelper;
            this.mapper = mapper;
            this.currentRequestAccessor = currentRequestAccessor;
        }

        public async Task<User> GetCurrentUserAsync()
        {
            var userId = currentRequestAccessor.Id;
            var user = await userRepository.GetUserByIdAsync(userId) ?? throw new KeyNotFoundException($"User with Id: '{userId}' was not found");

            return user;
        }

        public async Task CreateUserAsync(RegistrationDto userRegistration)
        {
            var existingUser = await userRepository.GetUserByEmailAsync(userRegistration.Email);

            if(existingUser != null)
            {
                throw new ArgumentException(message: $"User with email '{existingUser.Email}' already exists");
            }

            var newUser = mapper.Map<User>(userRegistration);
            var result = await userRepository.MapAndCreateUserAsync(newUser, userRegistration.Password);

            if (result.Succeeded)
            {
                await emailService.SendEmailConfimationLinkAsync(newUser.Email!);
            }
            else
            {
                throw new ArgumentException("User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        public async Task<LoginResponse> ValidateUserAsync(LoginDto userLogin, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.GetUserByEmailAsync(userLogin.Email, cancellationToken) ?? throw new AuthException($"No account for '{userLogin.Email}', please check your credentials and try again");
            var emailConfirmed = await userRepository.CheckIsEmailConfirmedForUserAsync(user);
            var validPassword = await userRepository.CheckPasswordForUserAsync(user, userLogin.Password!);

            if (!emailConfirmed)
            {
                await emailService.SendEmailConfimationLinkAsync(user.Email!);
                throw new AuthException("Please check your email and confirm your account before logging in");
            }

            if (!validPassword)
            {
                throw new AuthException("Invalid password for the given account");
            }

            var userRoles = await userRepository.GetRolesForUserAsync(user);
            var response = new LoginResponse
            {
                Token = await jwtHelper.CreateTokenAsync(user),
                Email = user.Email,
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = [.. userRoles],
            };

            return response;
        }

        public async Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.GetUserByEmailAsync(email, cancellationToken) ?? throw new KeyNotFoundException($"User with email: '{email}' was not found");

            return user;
        }

        public async Task<List<string>> GetRolesForEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await GetUserByEmailAsync(email, cancellationToken);
            var roles = await userRepository.GetRolesForUserAsync(user);

            return [.. roles];
        }

        public async Task<List<UserDto>> GetUsersAsync(Guid? exceptId = null, CancellationToken cancellationToken = default)
        {
            var users = await userRepository.GetUsersAsync(exceptId, cancellationToken);
            var userDtos = users.Select(u => mapper.Map<UserDto>(u)).ToList();

            return userDtos;
        }

        public async Task<string> GenerateEmailConfirmationTokenForUserAsync(User user)
        {
            return await userRepository.GenerateEmailConfirmationTokenForUserAsync(user);
        }

        public async Task<IdentityResult> ConfirmEmailForUserAsync(User user, string token)
        {
            return await userRepository.ConfirmEmailForUserAsync(user, token);
        }

        public async Task<UserDto> GetUserByIdAsync(Guid Id, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.GetUserByIdAsync(Id, cancellationToken) ?? throw new KeyNotFoundException($"User with Id: {Id} was not found");

            return mapper.Map<UserDto>(user);
        }

        public async Task<List<string>> GetRolesForUserAsync(User user)
        {
            var userRoles = await userRepository.GetRolesForUserAsync(user);

            return [.. userRoles];
        }

        public async Task<List<string>> UpdateUserRolesAsync(UpdateUserRolesRequest request, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.GetUserByIdAsync(request.UserId!.Value, cancellationToken) ?? throw new KeyNotFoundException($"User with Id: {request.UserId} was not found");
            var currentRoles = await userRepository.GetRolesForUserAsync(user);

            if(request.ActionType == ActionType.Create)
            {
                if (currentRoles.Contains(request.Role)) return [.. currentRoles];

                await userRepository.AddRoleToUserAsync(user, request.Role);
            }
            else if(request.ActionType == ActionType.Delete)
            {
                if (!currentRoles.Contains(request.Role)) return [.. currentRoles];
                
                await userRepository.RemoveRoleFromUserAsync(user, request.Role);
            }
            
            var updatedRoles = await userRepository.GetRolesForUserAsync(user);
            
            return [.. updatedRoles];
        }

        public async Task<List<AdminDashboardResponse>> GetAdminDashboardAsync(CancellationToken cancellationToken = default)
        {
            var adminId = currentRequestAccessor.Id;            
            var users = await userRepository.GetUsersAsync(adminId, cancellationToken);
            var result = new List<AdminDashboardResponse>();

            foreach (var user in users)
            {
                var roles = await userRepository.GetRolesForUserAsync(user);
                
                result.Add(new AdminDashboardResponse
                {
                    UserId = user.Id,
                    Email = user.Email!,
                    NumberOfPosts = user.Posts.Count,
                    LastPostedAt = user.Posts.MaxBy(p => p.LastUpdatedDate)?.LastUpdatedDate,
                    Roles = [.. roles]
                });
            }

            return result;
        }

        public async Task DeleteUserByIdAsync(Guid Id, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.GetUserByIdAsync(Id, cancellationToken) ?? throw new KeyNotFoundException($"User with Id: {Id} was not found");
            
            await userRepository.DeleteUserAsync(user);
        }
    }
}
