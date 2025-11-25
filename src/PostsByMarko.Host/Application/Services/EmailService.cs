using PostsByMarko.Host.Application.Exceptions;
using PostsByMarko.Host.Application.Helper;
using PostsByMarko.Host.Application.Interfaces;
using PostsByMarko.Host.Data.Repositories.Users;

namespace PostsByMarko.Host.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmailHelper emailHelper;
        private readonly IUserRepository userRepository;
        private readonly ICurrentRequestAccessor currentRequestAccessor;
        private readonly LinkGenerator linkGenerator;
        private const string CONFIRM_EMAIL_ENDPOINT_NAME = "ConfirmEmail";

        public EmailService(IEmailHelper emailHelper, IUserRepository userRepository, LinkGenerator linkGenerator, ICurrentRequestAccessor currentRequestAccessor)
        {
            this.emailHelper = emailHelper;
            this.userRepository = userRepository;
            this.linkGenerator = linkGenerator;
            this.currentRequestAccessor = currentRequestAccessor;
        }

        public async Task SendEmailConfimationLinkAsync(string emailToSendTo)
        {
            var user = await userRepository.GetUserByEmailAsync(emailToSendTo) ?? throw new KeyNotFoundException($"User with email '{emailToSendTo}' was not found");
            var token = await userRepository.GenerateEmailConfirmationTokenForUserAsync(user);
            var confirmationLink = GenerateEmailConfirmationLink(user.Email, token);
            var subject = $"Please confirm the registration for {user.Email}";
            var body = $"Your account has been successfully created. Please click on the following link to confirm your registration: {confirmationLink}";

            await emailHelper.SendEmailAsync(user.FirstName!, user.LastName!, user.Email, subject, body);
        }

        public async Task ConfirmEmailAsync(string email, string token)
        {
            var user = await userRepository.GetUserByEmailAsync(email) ?? throw new AuthException($"No account for '{email}', please check your credentials and try again");
            var emailConfirmed = await userRepository.ConfirmEmailForUserAsync(user, token);

            if (!emailConfirmed.Succeeded)
            {
                throw new AuthException("Error during email confirmation");
            }
        }

        private string GenerateEmailConfirmationLink(string email, string token)
        {
            var dictionaryValues = new RouteValueDictionary
            {
                { "email", email },
                { "token", token }
            };

            var confirmationLink = linkGenerator.GetUriByAddress(
                httpContext: currentRequestAccessor.requestContext,
                address: CONFIRM_EMAIL_ENDPOINT_NAME, // the route name
                values: dictionaryValues,
                scheme: null,
                host: null,
                pathBase: null,
                fragment: default,
                options: null
            );

            return confirmationLink!;
        }
    }
}
