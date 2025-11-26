using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Moq;
using PostsByMarko.Host.Application.Exceptions;
using PostsByMarko.Host.Application.Helper;
using PostsByMarko.Host.Application.Interfaces;
using PostsByMarko.Host.Application.Services;
using PostsByMarko.Host.Data.Entities;
using PostsByMarko.Host.Data.Repositories.Users;

namespace PostsByMarko.UnitTests
{
    public class EmailServiceTests
    {
        private readonly EmailService emailService;
        private readonly Mock<IEmailHelper> emailHelperMock = new();
        private readonly Mock<IUserRepository> userRepositoryMock = new();
        private readonly Mock<ICurrentRequestAccessor> currentRequestAccessorMock = new();
        private readonly Mock<LinkGenerator> linkGeneratorMock = new();

        public EmailServiceTests()
        {
            emailService = new EmailService(emailHelperMock.Object, userRepositoryMock.Object, linkGeneratorMock.Object, currentRequestAccessorMock.Object);
        }

        [Fact]
        public async Task send_email_confirmation_link_should_send_email_to_user()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "test@test.com", FirstName = "Test", LastName = "Test" };
            var token = "some_token";
            var dictionaryValues = new RouteValueDictionary
            {
                { "email", user.Email },
                { "token", token }
            };
            var confirmationLink = $"https://example.com/confirm?email={user.Email}&token={token}";
            var defaultHttpContext = new DefaultHttpContext();
            var expectedSubject = $"Please confirm the registration for {user.Email}";
            var expectedBody = $"Your account has been successfully created. Please click on the following link to confirm your registration: {confirmationLink}";

            currentRequestAccessorMock.Setup(c => c.requestContext).Returns(defaultHttpContext);
            userRepositoryMock.Setup(u => u.GetUserByEmailAsync(user.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            userRepositoryMock.Setup(u => u.GenerateEmailConfirmationTokenForUserAsync(user)).ReturnsAsync(token);
            linkGeneratorMock
                .Setup(lg => lg.GetUriByAddress(
                    defaultHttpContext,
                    "ConfirmEmail",
                    dictionaryValues,
                    null, null, null, default, default, null
                ))
                .Returns(confirmationLink);

            // Act
            await emailService.SendEmailConfimationLinkAsync(user.Email);

            // Assert
            emailHelperMock.Verify(e => e.SendEmailAsync(user.FirstName, user.LastName, user.Email, expectedSubject, expectedBody), Times.Once);
        }

        [Fact]
        public async Task send_email_confirmation_link_should_throw_if_email_was_not_found()
        {
            // Arrange
            var randomEmail = "test@test.com";

            // Act
            var result = async () => await emailService.SendEmailConfimationLinkAsync(randomEmail);

            // Assert
            await result.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"User with email '{randomEmail}' was not found");
        }

        [Fact]
        public async Task confirm_email_should_throw_if_email_was_not_found()
        {
            // Arrange
            var randomEmail = "test@test.com";
            var token = "some_token";

            // Act
            var result = async () => await emailService.ConfirmEmailAsync(randomEmail, token);

            // Assert
            await result.Should().ThrowAsync<AuthException>().WithMessage($"No account for '{randomEmail}', please check your credentials and try again");
        }

        [Fact]
        public async Task confirm_email_should_throw_if_confirmation_failed()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "test@test.com" };
            var failed = IdentityResult.Failed(new IdentityError());
            var token = "some_token";

            userRepositoryMock.Setup(u => u.GetUserByEmailAsync(user.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            userRepositoryMock.Setup(u => u.ConfirmEmailForUserAsync(user, token)).ReturnsAsync(failed);
            
            // Act
            var result = async () => await emailService.ConfirmEmailAsync(user.Email, token);

            // Assert
            await result.Should().ThrowAsync<AuthException>().WithMessage("Error during email confirmation");
        }
    }
}
