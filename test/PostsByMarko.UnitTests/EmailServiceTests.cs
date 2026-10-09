using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using PostsByMarko.Host.Application.Configuration;
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

        public EmailServiceTests()
        {
            var applicationUrls = Options.Create(new ApplicationUrlConfig
            {
                ApiBaseUrl = "https://example.com",
                ClientBaseUrl = "https://client.example.com"
            });
            emailService = new EmailService(emailHelperMock.Object, userRepositoryMock.Object, applicationUrls);
        }

        [Fact]
        public async Task send_email_confirmation_link_should_send_email_to_user()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "test@test.com", FirstName = "Test", LastName = "Test" };
            var token = "some_token";
            var confirmationLink = $"https://example.com/api/auth/confirm?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}";
            var expectedSubject = $"Please confirm the registration for {user.Email}";
            EmailContent? delivered = null;

            userRepositoryMock.Setup(u => u.GetUserByEmailAsync(user.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            userRepositoryMock.Setup(u => u.GenerateEmailConfirmationTokenForUserAsync(user)).ReturnsAsync(token);
            emailHelperMock.Setup(helper => helper.SendEmailAsync(
                user.FirstName, user.LastName, user.Email, expectedSubject,
                It.IsAny<EmailContent>(), It.IsAny<CancellationToken>()))
                .Callback<string, string, string, string, EmailContent, CancellationToken>(
                    (_, _, _, _, content, _) => delivered = content)
                .Returns(Task.CompletedTask);

            // Act
            await emailService.SendEmailConfirmationLinkAsync(user.Email);

            // Assert
            emailHelperMock.Verify(e => e.SendEmailAsync(
                user.FirstName, user.LastName, user.Email, expectedSubject, It.IsAny<EmailContent>(), It.IsAny<CancellationToken>()), Times.Once);
            delivered.Should().NotBeNull();
            delivered!.TextBody.Should().Contain(confirmationLink);
            delivered.HtmlBody.Should().Contain("Confirm your email</a>");
            delivered.HtmlBody.Should().Contain(System.Text.Encodings.Web.HtmlEncoder.Default.Encode(confirmationLink));
        }

        [Fact]
        public async Task send_email_confirmation_link_should_throw_if_email_was_not_found()
        {
            // Arrange
            var randomEmail = "test@test.com";

            // Act
            var result = async () => await emailService.SendEmailConfirmationLinkAsync(randomEmail);

            // Assert
            await result.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"User with email '{randomEmail}' was not found");
        }

        [Fact]
        public async Task send_email_confirmation_link_should_skip_already_confirmed_user()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "test@test.com" };
            userRepositoryMock.Setup(repository => repository.GetUserByEmailAsync(
                user.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            userRepositoryMock.Setup(repository => repository.CheckIsEmailConfirmedForUserAsync(user)).ReturnsAsync(true);

            // Act
            await emailService.SendEmailConfirmationLinkAsync(user.Email);

            // Assert
            emailHelperMock.Verify(helper => helper.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<EmailContent>(),
                It.IsAny<CancellationToken>()), Times.Never);
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
            await result.Should().ThrowAsync<BadRequestException>()
                .WithMessage("The confirmation link is invalid or has expired.");
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
            await result.Should().ThrowAsync<BadRequestException>()
                .WithMessage("The confirmation link is invalid or has expired.");
        }
    }
}
