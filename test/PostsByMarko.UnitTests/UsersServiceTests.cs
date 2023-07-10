using FluentAssertions;
using Moq;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Host.Data.Models.Dtos;
using PostsByMarko.Host.Data.Models.Responses;
using PostsByMarko.Host.Data.Repos.Users;
using PostsByMarko.Host.Helper;
using PostsByMarko.Host.Services;
using System.Net;

namespace PostsByMarko.UnitTests
{
    public class UsersServiceTests
    {
        private readonly UsersService service;
        private readonly Mock<IUsersRepository> usersRepositoryMock = new Mock<IUsersRepository>();
        private readonly Mock<IJwtHelper> jwtHelperMock = new Mock<IJwtHelper>();


        public UsersServiceTests()
        {
            service = new UsersService(usersRepositoryMock.Object, jwtHelperMock.Object);
        }

        [Fact]
        public async Task map_and_create_user_should_return_created_with_appropriate_message_when_user_is_registered()
        {
            // Arrange
            var userToRegister = new UserRegistrationDto
            {
                Email = "some_user@test.com",
                Password = "test_password"
            };

            usersRepositoryMock.Setup(r => r.MapAndCreateUserAsync(It.IsAny<User>(), userToRegister.Password)).ReturnsAsync(() => true);

            // Act
            var result = await service.MapAndCreateUserAsync(userToRegister);

            // Assert
            result.Message.Should().Be("Successfully Registered");
            result.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task map_and_create_user_should_return_bad_request_with_appropriate_message_when_user_is_not_registered()
        {
            // Arrange
            var userToRegister = new UserRegistrationDto
            {
                Email = "test_user@test.com",
                Password = "test_password"
            };
            var registeredUser = new User(userToRegister.Email);

            usersRepositoryMock.Setup(r => r.MapAndCreateUserAsync(registeredUser, userToRegister.Password)).ReturnsAsync(() => false);

            // Act
            var result = await service.MapAndCreateUserAsync(userToRegister);

            // Assert
            result.Message.Should().Be("Error during user registration");
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task validate_user_should_return_ok_with_appropriate_message_and_payload_when_user_is_logged_in()
        {
            // Arrange
            var user = new User("test_user@test.com", "Test", "User");
            var userLogin = new UserLoginDto { Email = user.Email, Password = "test_password" };
            var jwtToken = "some_jwt_token";
            var userRoles = new List<string> { "User" };
            var loginResponse = new LoginResponse { Token = jwtToken, Email = user.Email, FirstName = user.FirstName, LastName = user.LastName, UserId = user.Id, Roles = userRoles };

            jwtHelperMock.Setup(h => h.CreateTokenAsync(user)).ReturnsAsync(jwtToken);
            usersRepositoryMock.Setup(r => r.GetRolesForEmailAsync(user.Email)).ReturnsAsync(userRoles);
            usersRepositoryMock.Setup(r => r.GetUserByEmailAsync(user.Email)).ReturnsAsync(user);
            usersRepositoryMock.Setup(r => r.CheckPasswordForUserAsync(user, userLogin.Password)).ReturnsAsync(() => true);
            usersRepositoryMock.Setup(r => r.CheckIsEmailConfirmedForUserAsync(user)).ReturnsAsync(() => true);

            // Act
            var result = await service.ValidateUserAsync(userLogin);
            var resultPayload = result.Payload as LoginResponse;

            // Assert
            resultPayload.Should().BeEquivalentTo(loginResponse);
            result.Message.Should().Be("Successfully Logged In");
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task validate_user_should_return_bad_request_with_appropriate_message_when_user_does_not_exist()
        {
            // Arrange
            var userLogin = new UserLoginDto { Email = "test_user", Password = "test_password" };

            usersRepositoryMock.Setup(r => r.GetUserByEmailAsync(It.IsAny<string>())).ReturnsAsync(() => null);

            // Act
            var result = await service.ValidateUserAsync(userLogin);

            // Assert
            result.Message.Should().Be("No account found, please check your credentials and try again");
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task validate_user_should_return_bad_request_with_appropriate_message_when_password_does_not_match()
        {
            // Arrange
            var user = new User("test_user@test.com");
            var userLogin = new UserLoginDto { Email = "test_user@test.com", Password = "test_password" };

            usersRepositoryMock.Setup(r => r.GetUserByEmailAsync(user.Email)).ReturnsAsync(user);
            usersRepositoryMock.Setup(r => r.CheckPasswordForUserAsync(user, userLogin.Password)).ReturnsAsync(() => false);

            // Act
            var result = await service.ValidateUserAsync(userLogin);

            // Assert
            result.Message.Should().Be("Invalid password for the given account");
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task validate_user_should_return_forbidden_with_appropriate_message_when_email_is_not_confirmed()
        {
            // Arrange
            var user = new User("test_user@test.com");
            var userLogin = new UserLoginDto { Email = "test_user@test.com", Password = "test_password" };

            usersRepositoryMock.Setup(r => r.GetUserByEmailAsync(user.Email)).ReturnsAsync(user);
            usersRepositoryMock.Setup(r => r.CheckPasswordForUserAsync(user, userLogin.Password)).ReturnsAsync(() => true);
            usersRepositoryMock.Setup(r => r.CheckIsEmailConfirmedForUserAsync(user)).ReturnsAsync(() => false);

            // Act
            var result = await service.ValidateUserAsync(userLogin);

            // Assert
            result.Message.Should().Be("Please check your email and confirm your account before logging in");
            result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task get_user_by_username_should_return_user()
        {
            // Arrange
            var user = new User("test_user@test.com");

            usersRepositoryMock.Setup(r => r.GetUserByEmailAsync(user.Email)).ReturnsAsync(user);

            // Act
            var result = await service.GetUserByEmailAsync(user.Email);

            // Assert
            result.Should().BeEquivalentTo(user);
        }


        [Fact]
        public async Task get_all_usernames_should_return_ok_with_payload_of_all_usernames()
        {
            // Arrange
            var usernames = new List<string> { "test_user", "other_user", "one_more_user" };

            usersRepositoryMock.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(usernames);

            // Act
            var result = await service.GetAllUsersAsync();
            var resultPayload = result.Payload as List<string>;

            // Assert
            resultPayload.Should().BeEquivalentTo(usernames);
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task generate_email_confirmation_token_for_user_should_return_token()
        {
            // Arrange
            var token = "some_token";
            var user = new User("test_user@test.com");

            usersRepositoryMock.Setup(r => r.GenerateEmailConfirmationTokenForUserAsync(user)).ReturnsAsync(token);

            // Act
            var result = await service.GenerateEmailConfirmationTokenForUserAsync(user);

            // Assert
            result.Should().Be(token);
        }

        [Fact]
        public async Task confirm_email_for_user_should_return_flag_whether_email_is_confirmed()
        {
            // Arrange
            var token = "some_token";
            var user = new User("test_user@test.com");

            usersRepositoryMock.Setup(r => r.ConfirmEmailForUserAsync(user, token)).ReturnsAsync(() => true);

            // Act
            var result = await service.ConfirmEmailForUserAsync(user, token);

            // Assert
            result.Should().BeTrue();
        }

    }
}
