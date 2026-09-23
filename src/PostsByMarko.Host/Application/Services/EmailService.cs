using PostsByMarko.Host.Application.Exceptions;
using PostsByMarko.Host.Application.Helper;
using PostsByMarko.Host.Application.Interfaces;
using PostsByMarko.Host.Data.Repositories.Users;
using Microsoft.Extensions.Options;
using PostsByMarko.Host.Application.Configuration;

namespace PostsByMarko.Host.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmailHelper emailHelper;
        private readonly IUserRepository userRepository;
        private readonly ApplicationUrlConfig applicationUrls;

        public EmailService(IEmailHelper emailHelper, IUserRepository userRepository, IOptions<ApplicationUrlConfig> applicationUrls)
        {
            this.emailHelper = emailHelper;
            this.userRepository = userRepository;
            this.applicationUrls = applicationUrls.Value;
        }

        public async Task SendEmailConfirmationLinkAsync(string emailToSendTo, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.GetUserByEmailAsync(emailToSendTo, cancellationToken) ?? throw new KeyNotFoundException($"User with email '{emailToSendTo}' was not found");
            if (await userRepository.CheckIsEmailConfirmedForUserAsync(user))
            {
                return;
            }

            var token = await userRepository.GenerateEmailConfirmationTokenForUserAsync(user);
            var confirmationLink = GenerateEmailConfirmationLink(user.Email!, token);
            var subject = $"Please confirm the registration for {user.Email}";
            var body = $"Your account has been successfully created. Please click on the following link to confirm your registration and sign in: {confirmationLink}";

            await emailHelper.SendEmailAsync(user.FirstName!, user.LastName!, user.Email!, subject, body, cancellationToken);
        }

        public async Task ConfirmEmailAsync(string email, string token)
        {
            var user = await userRepository.GetUserByEmailAsync(email) ?? throw new BadRequestException("The confirmation link is invalid or has expired.");
            var emailConfirmed = await userRepository.ConfirmEmailForUserAsync(user, token);

            if (!emailConfirmed.Succeeded)
            {
                throw new BadRequestException("The confirmation link is invalid or has expired.");
            }
        }

        private string GenerateEmailConfirmationLink(string email, string token)
        {
            if (!Uri.TryCreate(applicationUrls.ApiBaseUrl, UriKind.Absolute, out var apiBaseUrl))
            {
                throw new InvalidOperationException("ApplicationUrls:ApiBaseUrl must be an absolute URL.");
            }

            var path = "/api/auth/confirm";
            var query = $"?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";
            var confirmationLink = new Uri(apiBaseUrl, $"{path}{query}").ToString();

            return confirmationLink;
        }
    }
}
