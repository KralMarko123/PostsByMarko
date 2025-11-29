using Moq;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Exceptions;
using PostsByMarko.Host.Application.Helper;
using PostsByMarko.Host.Application.Interfaces;
using PostsByMarko.Host.Application.Services;
using PostsByMarko.Host.Data.Entities;
using PostsByMarko.Host.Data.Repositories.Users;
using PostsByMarko.Host.Application.Enums;
using PostsByMarko.Host.Application.Requests;

namespace PostsByMarko.UnitTests
{
    public class UserServiceTests
    {
        private readonly UserService userService;
        private readonly Mock<IUserRepository> usersRepositoryMock = new();
        private readonly Mock<IEmailService> emailServiceMock = new();
        private readonly Mock<IJwtHelper> jwtHelperMock = new();
        private readonly Mock<IMapper> mapperMock = new();
        private readonly Mock<ICurrentRequestAccessor> currentRequestAccessorMock = new();

        public UserServiceTests()
        {
            userService = new UserService(usersRepositoryMock.Object, emailServiceMock.Object, jwtHelperMock.Object,
                mapperMock.Object, currentRequestAccessorMock.Object);
        }

        [Fact]
        public async Task get_current_user_should_return_the_current_user()
        {
            // Arrange
            var user = new User() { Id = Guid.NewGuid() };

            currentRequestAccessorMock.Setup(a => a.Id).Returns(user.Id);
            usersRepositoryMock.Setup(r => r.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(() => user);

            // Act
            var result = await userService.GetCurrentUserAsync();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(user.Id);
        }

        [Fact]
        public async Task get_current_user_should_throw_if_user_was_not_found()
        {
            // Arrange
            var randomId = Guid.NewGuid();

            currentRequestAccessorMock.Setup(a => a.Id).Returns(randomId);

            // Act
            var result = async () => await userService.GetCurrentUserAsync();

            // Assert
            await result.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"User with Id: '{randomId}' was not found");
        }

        [Fact]
        public async Task create_user_should_map_and_create_user_and_send_email_confirmation_link()
        {
            // Arrange
            var identityResult = IdentityResult.Success;
            var user = new User() { Id = Guid.NewGuid(), Email = "test@test.com" };
            var registrationDto = new RegistrationDto
            {
                Password = "Password"
            };

            mapperMock.Setup(m => m.Map<User>(registrationDto)).Returns(user);
            usersRepositoryMock.Setup(r => r.MapAndCreateUserAsync(user, registrationDto.Password)).ReturnsAsync(() => identityResult);

            // Act
            await userService.CreateUserAsync(registrationDto);

            // Assert
            emailServiceMock.Verify(s => s.SendEmailConfimationLinkAsync(user.Email), Times.Once);
        }

        [Fact]
        public async Task create_user_should_throw_if_user_already_exists()
        {
            // Arrange
            var user = new User() { Id = Guid.NewGuid(), Email = "test@test.com" };
            var registrationDto = new RegistrationDto
            {
                Email = user.Email,
                Password = "Password"
            };

            usersRepositoryMock.Setup(r => r.GetUserByEmailAsync(registrationDto.Email, It.IsAny<CancellationToken>())).ReturnsAsync(() => user);

            // Act
            var result = async () => await userService.CreateUserAsync(registrationDto);

            // Assert
            await result.Should().ThrowAsync<ArgumentException>().WithMessage($"User with email '{registrationDto.Email}' already exists");
        }

        [Fact]
        public async Task create_user_should_throw_if_user_failed_to_create()
        {
            // Arrange
            var identityResult = IdentityResult.Failed(new IdentityError { Description = "Some error" });
            var user = new User() { Id = Guid.NewGuid(), Email = "test@test.com" };
            var registrationDto = new RegistrationDto
            {
                Password = "Password"
            };

            mapperMock.Setup(m => m.Map<User>(registrationDto)).Returns(user);
            usersRepositoryMock.Setup(r => r.MapAndCreateUserAsync(user, registrationDto.Password)).ReturnsAsync(() => identityResult);

            // Act
            var result = async () => await userService.CreateUserAsync(registrationDto);

            // Assert
            await result.Should().ThrowAsync<ArgumentException>().WithMessage($"User creation failed: {identityResult.Errors.First().Description}");
        }

        [Fact]
        public async Task validate_user_should_return_login_response()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@test.com",
                Password = "Password"
            };

            var user = new User() { Id = Guid.NewGuid(), Email = loginDto.Email, FirstName = "Test", LastName = "Test" };

            usersRepositoryMock.Setup(r => r.GetUserByEmailAsync(loginDto.Email, It.IsAny<CancellationToken>())).ReturnsAsync(() => user);
            usersRepositoryMock.Setup(r => r.CheckIsEmailConfirmedForUserAsync(user)).ReturnsAsync(() => true);
            usersRepositoryMock.Setup(r => r.CheckPasswordForUserAsync(user, loginDto.Password)).ReturnsAsync(() => true);
            usersRepositoryMock.Setup(r => r.GetRolesForUserAsync(user)).ReturnsAsync(["Some Role"]);
            jwtHelperMock.Setup(j => j.CreateTokenAsync(user)).ReturnsAsync("token");

            // Act
            var result = await userService.ValidateUserAsync(loginDto);

            // Assert
            result.Should().NotBeNull();
            result.Token.Should().Be("token");
            result.Id.Should().Be(user.Id);
            result.Email.Should().Be(user.Email);
            result.Roles.Should().ContainSingle().Which.Should().Be("Some Role");
            result.FirstName.Should().Be(user.FirstName);
            result.LastName.Should().Be(user.LastName);
        }

        [Fact]
        public async Task validate_user_should_throw_if_user_is_not_found()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@test.com",
                Password = "Password"
            };

            // Act
            var result = async () => await userService.ValidateUserAsync(loginDto);

            // Assert
            await result.Should().ThrowAsync<AuthException>().WithMessage($"No account for '{loginDto.Email}', please check your credentials and try again");
        }

        [Fact]
        public async Task validate_user_should_throw_and_send_confirmation_link_if_email_is_not_confirmed()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@test.com",
                Password = "Password"
            };

            var user = new User() { Id = Guid.NewGuid(), Email = loginDto.Email, FirstName = "Test", LastName = "Test" };

            usersRepositoryMock.Setup(r => r.GetUserByEmailAsync(loginDto.Email, It.IsAny<CancellationToken>())).ReturnsAsync(() => user);
            usersRepositoryMock.Setup(r => r.CheckIsEmailConfirmedForUserAsync(user)).ReturnsAsync(() => false);
            usersRepositoryMock.Setup(r => r.CheckPasswordForUserAsync(user, loginDto.Password)).ReturnsAsync(() => true);

            // Act
            var result = async () => await userService.ValidateUserAsync(loginDto);

            // Assert
            await result.Should().ThrowAsync<AuthException>().WithMessage("Please check your email and confirm your account before logging in");
            emailServiceMock.Verify(s => s.SendEmailConfimationLinkAsync(user.Email), Times.Once);
        }

        [Fact]
        public async Task validate_user_should_throw_if_password_is_not_valid()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@test.com",
                Password = "Password"
            };

            var user = new User() { Id = Guid.NewGuid(), Email = loginDto.Email, FirstName = "Test", LastName = "Test" };

            usersRepositoryMock.Setup(r => r.GetUserByEmailAsync(loginDto.Email, It.IsAny<CancellationToken>())).ReturnsAsync(() => user);
            usersRepositoryMock.Setup(r => r.CheckIsEmailConfirmedForUserAsync(user)).ReturnsAsync(() => true);
            usersRepositoryMock.Setup(r => r.CheckPasswordForUserAsync(user, loginDto.Password)).ReturnsAsync(() => false);

            // Act
            var result = async () => await userService.ValidateUserAsync(loginDto);

            // Assert
            await result.Should().ThrowAsync<AuthException>().WithMessage("Invalid password for the given account");
        }

        [Fact]
        public async Task get_user_by_email_should_return_user()
        {
            // Arrange
            var user = new User() { Id = Guid.NewGuid(), Email = "test@test.com" };

            usersRepositoryMock.Setup(r => r.GetUserByEmailAsync(user.Email, It.IsAny<CancellationToken>())).ReturnsAsync(() => user);

            // Act
            var result = await userService.GetUserByEmailAsync(user.Email);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(user);
        }

        [Fact]
        public async Task get_user_by_email_should_throw_if_user_was_not_found()
        {
            // Arrange
            var randomEmail = "test@test.com";

            // Act
            var result = async () => await userService.GetUserByEmailAsync(randomEmail);

            // Assert
            await result.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"User with email: '{randomEmail}' was not found");
        }

        [Fact]
        public async Task get_roles_for_email_should_return_user_roles()
        {
            // Arrange
            var user = new User() { Id = Guid.NewGuid(), Email = "test@test.com" };
            var roles = new List<string> { "Role1", "Role2" };

            usersRepositoryMock.Setup(r => r.GetUserByEmailAsync(user.Email, It.IsAny<CancellationToken>())).ReturnsAsync(() => user);
            usersRepositoryMock.Setup(r => r.GetRolesForUserAsync(user)).ReturnsAsync(() => roles);

            // Act
            var result = await userService.GetRolesForEmailAsync(user.Email);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(roles);
        }

        [Fact]
        public async Task get_users_should_return_users()
        {
            // Arrange
            var users = new List<User>
            {
                new User() { Id = Guid.NewGuid() },
                new User() { Id = Guid.NewGuid() }
            };

            var usersDto = new List<UserDto>
            {
                new UserDto() { Id = users[0].Id },
                new UserDto() { Id = users[1].Id }
            };

            mapperMock.Setup(m => m.Map<UserDto>(users[0])).Returns(usersDto[0]);
            mapperMock.Setup(m => m.Map<UserDto>(users[1])).Returns(usersDto[1]);
            usersRepositoryMock.Setup(r => r.GetUsersAsync(null, It.IsAny<CancellationToken>())).ReturnsAsync(() => users);

            // Act
            var result = await userService.GetUsersAsync(null, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(usersDto.Count);
            result.Should().BeEquivalentTo(usersDto);
        }

        [Fact]
        public async Task get_users_should_return_filtered_users_when_provided_except_id()
        {
            // Arrange
            var users = new List<User>
            {
                new User() { Id = Guid.NewGuid() },
                new User() { Id = Guid.NewGuid() }
            };

            var usersDto = new List<UserDto>
            {
                new UserDto() { Id = users[0].Id },
                new UserDto() { Id = users[1].Id }
            };

            mapperMock.Setup(m => m.Map<UserDto>(users[0])).Returns(usersDto[0]);
            mapperMock.Setup(m => m.Map<UserDto>(users[1])).Returns(usersDto[1]);
            usersRepositoryMock.Setup(r => r.GetUsersAsync(users[1].Id, It.IsAny<CancellationToken>())).ReturnsAsync(() => [users[0]]);

            // Act
            var result = await userService.GetUsersAsync(users[1].Id, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result.Should().BeEquivalentTo([usersDto[0]]);
        }

        [Fact]
        public async Task generate_email_confirmation_token_for_user_should_return_token()
        {
            // Arrange
            var user = new User() { Id = Guid.NewGuid() };

            usersRepositoryMock.Setup(r => r.GenerateEmailConfirmationTokenForUserAsync(user)).ReturnsAsync(() => "token");

            // Act
            var result = await userService.GenerateEmailConfirmationTokenForUserAsync(user);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be("token");
        }

        [Fact]
        public async Task confirm_email_for_user_should_return_identity_result()
        {
            // Arrange
            var identityResult = IdentityResult.Success;
            var user = new User() { Id = Guid.NewGuid() };

            usersRepositoryMock.Setup(r => r.ConfirmEmailForUserAsync(user, "token")).ReturnsAsync(() => identityResult);

            // Act
            var result = await userService.ConfirmEmailForUserAsync(user, "token");

            // Assert
            result.Should().NotBeNull();
            result.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task get_user_by_id_should_return_user()
        {
            // Arrange
            var user = new User() { Id = Guid.NewGuid() };
            var userDto = new UserDto { Id = user.Id };

            mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);
            usersRepositoryMock.Setup(r => r.GetUserByIdAsync(userDto.Id.Value, It.IsAny<CancellationToken>())).ReturnsAsync(() => user);

            // Act
            var result = await userService.GetUserByIdAsync(user.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(userDto);
        }

        [Fact]
        public async Task get_user_by_id_should_throw_if_user_was_not_found()
        {
            // Arrange
            var randomId = Guid.NewGuid();

            // Act
            var result = async () => await userService.GetUserByIdAsync(randomId);

            // Assert
            await result.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"User with Id: {randomId} was not found");
        }

        [Fact]
        public async Task get_roles_for_user_should_return_roles()
        {
            // Arrange
            var user = new User() { Id = Guid.NewGuid() };
            var roles = new List<string> { "Role1", "Role2" };

            usersRepositoryMock.Setup(r => r.GetRolesForUserAsync(user)).ReturnsAsync(() => roles);

            // Act
            var result = await userService.GetRolesForUserAsync(user);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(roles);
        }

        [Fact]
        public async Task update_user_roles_should_return_with_new_role()
        {
            // Arrange
            var user = new User() { Id = Guid.NewGuid() };
            var request = new UpdateUserRolesRequest
            {
                UserId = user.Id,
                ActionType = ActionType.Create,
                Role = "New role"
            };
            var roles = new List<string> { "Some role" };

            usersRepositoryMock.Setup(r => r.GetUserByIdAsync(request.UserId.Value, It.IsAny<CancellationToken>())).ReturnsAsync(() => user);
            usersRepositoryMock.Setup(r => r.GetRolesForUserAsync(user)).ReturnsAsync(() => roles);

            // Act
            var result = await userService.UpdateUserRolesAsync(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            usersRepositoryMock.Verify(r => r.AddRoleToUserAsync(user, request.Role), Times.Once);
        }

        [Fact]
        public async Task update_user_roles_should_return_with_removed_role()
        {
            // Arrange
            var user = new User() { Id = Guid.NewGuid() };
            var request = new UpdateUserRolesRequest
            {
                UserId = user.Id,
                ActionType = ActionType.Delete,
                Role = "Removed role"
            };
            var roles = new List<string> { "Some role", request.Role };

            usersRepositoryMock.Setup(r => r.GetUserByIdAsync(request.UserId.Value, It.IsAny<CancellationToken>())).ReturnsAsync(() => user);
            usersRepositoryMock.Setup(r => r.GetRolesForUserAsync(user)).ReturnsAsync(() => roles);

            // Act
            var result = await userService.UpdateUserRolesAsync(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            usersRepositoryMock.Verify(r => r.RemoveRoleFromUserAsync(user, request.Role), Times.Once);
        }

        [Fact]
        public async Task update_user_roles_should_throw_if_user_was_not_found()
        {
            // Arrange
            var request = new UpdateUserRolesRequest
            {
                UserId = Guid.NewGuid(),
            };

            // Act
            var result = async () => await userService.UpdateUserRolesAsync(request, CancellationToken.None);

            // Assert
            await result.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"User with Id: {request.UserId} was not found");
        }

        [Fact]
        public async Task delete_user_should_call_delete_method_in_repository()
        {
            // Arrange
            var user = new User() { Id = Guid.NewGuid() };

            usersRepositoryMock.Setup(r => r.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(() => user);

            // Act
            await userService.DeleteUserByIdAsync(user.Id, CancellationToken.None);

            // Assert
            usersRepositoryMock.Verify(r => r.DeleteUserAsync(user), Times.Once);
        }

        [Fact]
        public async Task delete_user_should_throw_if_user_was_not_found()
        {
            // Arrange
            var randomId = Guid.NewGuid();

            // Act
            var result = async () => await userService.DeleteUserByIdAsync(randomId, CancellationToken.None);

            // Assert
            await result.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"User with Id: {randomId} was not found");
        }

        [Fact]
        public async Task get_admin_dashboard_should_return_dashboard_data()
        {
            // Arrange
            var admin = new User() { Id = Guid.NewGuid() };
            var today = DateTime.Now;
            var users = new List<User>() 
            { 
                new User { Id = Guid.NewGuid() },
                new User 
                {
                    Id = Guid.NewGuid(), 
                    Email = "test@test.com",
                    Posts = new List<Post>
                    {
                        new Post { Id = Guid.NewGuid(), LastUpdatedDate = today },
                        new Post { Id = Guid.NewGuid() }
                    }
                }
            };
            var userRoles = new List<string> { "Some role" };

            currentRequestAccessorMock.Setup(a => a.Id).Returns(admin.Id);
            usersRepositoryMock.Setup(r => r.GetUsersAsync(admin.Id, It.IsAny<CancellationToken>())).ReturnsAsync(() => users);
            usersRepositoryMock.Setup(r => r.GetRolesForUserAsync(It.IsAny<User>())).ReturnsAsync(() => userRoles);

            // Act
            var result = await userService.GetAdminDashboardAsync(CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(users.Count);

            var userWithData = result[1];

            userWithData.UserId.Should().Be(users[1].Id);
            userWithData.Email.Should().Be(users[1].Email);
            userWithData.NumberOfPosts.Should().Be(2);
            userWithData.LastPostedAt.Should().BeWithin(TimeSpan.FromSeconds(5));
            userWithData.Roles.Should().BeEquivalentTo(userRoles);
        }
    }
}
