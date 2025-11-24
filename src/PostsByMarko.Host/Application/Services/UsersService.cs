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
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository usersRepository;
        private readonly IEmailService emailService;
        private readonly IJwtHelper jwtHelper;
        private readonly IMapper mapper;
        private readonly ICurrentRequestAccessor currentRequestAccessor;

        public UsersService(IUsersRepository usersRepository, IEmailService emailService, IJwtHelper jwtHelper, IMapper mapper, ICurrentRequestAccessor currentRequestAccessor)
        {
            this.usersRepository = usersRepository;
            this.emailService = emailService;
            this.jwtHelper = jwtHelper;
            this.mapper = mapper;
            this.currentRequestAccessor = currentRequestAccessor;
        }

        public async Task<User> GetCurrentUserAsync()
        {
            var userId = Guid.Parse(currentRequestAccessor.Id);
            var user = await usersRepository.GetUserByIdAsync(userId) ?? throw new KeyNotFoundException($"User with Id: '{userId}' was not found");

            return user;
        }

        public async Task CreateUserAsync(RegistrationDto registrationDto)
        {
            var newUser = mapper.Map<User>(registrationDto);
            var result = await usersRepository.MapAndCreateUserAsync(newUser, registrationDto.Password);

            if (result.Succeeded)
            {
                await emailService.SendEmailConfimationLinkAsync(newUser.Email);
            }
            else
            {
                throw new AuthException("User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        public async Task<LoginResponse> ValidateUserAsync(LoginDto userLogin)
        {
            var user = await usersRepository.GetUserByEmailAsync(userLogin.Email) ?? throw new AuthException($"No account for '{userLogin.Email}', please check your credentials and try again");
            var emailConfirmed = await usersRepository.CheckIsEmailConfirmedForUserAsync(user);
            var validPassword = await usersRepository.CheckPasswordForUserAsync(user, userLogin.Password!);

            if (!emailConfirmed)
            {
                await emailService.SendEmailConfimationLinkAsync(user.Email);
                throw new AuthException("Please check your email and confirm your account before logging in");
            }

            if (!validPassword)
            {
                throw new AuthException("Invalid password for the given account");
            }

            var userRoles = await usersRepository.GetRolesForUserAsync(user);
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

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await usersRepository.GetUserByEmailAsync(email) ?? throw new KeyNotFoundException($"User with email: '{email}' was not found");

            return user;
        }

        public async Task<List<string>> GetRolesForEmailAsync(string email)
        {
            var user = await GetUserByEmailAsync(email);
            var roles = await usersRepository.GetRolesForUserAsync(user);

            return [.. roles];
        }

        public async Task<List<UserDto>> GetUsersAsync(Guid? exceptId = null, CancellationToken cancellationToken = default)
        {
            var users = await usersRepository.GetUsersAsync(exceptId, cancellationToken);
            var userDtos = users.Select(u => mapper.Map<UserDto>(u)).ToList();

            return userDtos;
        }

        public async Task<string> GenerateEmailConfirmationTokenForUserAsync(User user)
        {
            return await usersRepository.GenerateEmailConfirmationTokenForUserAsync(user);
        }

        public async Task<IdentityResult> ConfirmEmailForUserAsync(User user, string token)
        {
            return await usersRepository.ConfirmEmailForUserAsync(user, token);
        }

        public async Task<UserDto> GetUserByIdAsync(Guid id)
        {
            var user = await usersRepository.GetUserByIdAsync(id) ?? throw new KeyNotFoundException($"User with Id: {id} was not found");

            return mapper.Map<UserDto>(user);
        }

        public async Task<List<string>> GetRolesForUserAsync(User user)
        {
            var userRoles = await usersRepository.GetRolesForUserAsync(user);

            return [.. userRoles];
        }

        public async Task<List<string>> UpdateUserRolesAsync(UpdateUserRolesRequest request, CancellationToken cancellationToken = default)
        {
            var user = await usersRepository.GetUserByIdAsync(request.UserId) ?? throw new KeyNotFoundException($"User with Id: {request.UserId} was not found");
            var currentRoles = await usersRepository.GetRolesForUserAsync(user);

            if(request.ActionType == ActionType.Create)
            {
                if (currentRoles.Contains(request.Role)) return [.. currentRoles];

                await usersRepository.AddRoleToUserAsync(user, request.Role);
            }
            else if(request.ActionType == ActionType.Delete)
            {
                if (!currentRoles.Contains(request.Role)) return [.. currentRoles];
                
                await usersRepository.RemoveRoleFromUserAsync(user, request.Role);
            }
            
            var updatedRoles = await usersRepository.GetRolesForUserAsync(user);
            
            return [.. updatedRoles];
        }

        public async Task<List<AdminDashboardResponse>> GetAdminDashboardAsync(CancellationToken cancellationToken = default)
        {
            var adminId = Guid.Parse(currentRequestAccessor.Id);            
            var users = await usersRepository.GetUsersAsync(adminId, cancellationToken);
            var result = new List<AdminDashboardResponse>();

            foreach (var user in users)
            {
                var roles = await usersRepository.GetRolesForUserAsync(user);
                
                result.Add(new AdminDashboardResponse
                {
                    UserId = user.Id,
                    Email = user.Email,
                    NumberOfPosts = user.Posts.Count,
                    LastPostedAt = user.Posts.MaxBy(p => p.LastUpdatedDate)?.LastUpdatedDate,
                    Roles = [.. roles]
                });
            }

            return result;
        }

        public async Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await usersRepository.GetUserByIdAsync(id) ?? throw new KeyNotFoundException($"User with Id: {id} was not found");
            
            await usersRepository.DeleteUserAsync(user);
        }
    }
}
