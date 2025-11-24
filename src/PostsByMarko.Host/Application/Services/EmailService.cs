using PostsByMarko.Host.Application.Exceptions;
using PostsByMarko.Host.Application.Helper;
using PostsByMarko.Host.Application.Interfaces;

namespace PostsByMarko.Host.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmailHelper emailHelper;
        private readonly IUserService usersService;
        private readonly ICurrentRequestAccessor currentRequestAccessor;
        private readonly LinkGenerator linkGenerator;
        private const string CONFIRM_EMAIL_ENDPOINT_NAME = "ConfirmEmail";

        public EmailService(IEmailHelper emailHelper, IUserService usersService, LinkGenerator linkGenerator, ICurrentRequestAccessor currentRequestAccessor)
        {
            this.emailHelper = emailHelper;
            this.usersService = usersService;
            this.linkGenerator = linkGenerator;
            this.currentRequestAccessor = currentRequestAccessor;
        }

        public async Task SendEmailConfimationLinkAsync(string emailToSendTo)
        {
            var user = await usersService.GetUserByEmailAsync(emailToSendTo);
            var token = await usersService.GenerateEmailConfirmationTokenForUserAsync(user);
            var confirmationLink = GenerateEmailConfirmationLink(user.Email, token);
            var subject = $"Please confirm the registration for {user.Email}";
            var body = $"Your account has been successfully created. Please click on the following link to confirm your registration: {confirmationLink}";

            await emailHelper.SendEmailAsync(user.FirstName!, user.LastName!, user.Email, subject, body);
        }

        public async Task ConfirmEmailAsync(string email, string token)
        {
            var user = await usersService.GetUserByEmailAsync(email) ?? throw new AuthException($"No account for '{email}', please check your credentials and try again");
            var emailConfirmed = await usersService.ConfirmEmailForUserAsync(user, token);

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
