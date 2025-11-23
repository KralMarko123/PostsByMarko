using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Exceptions;
using PostsByMarko.Host.Application.Helper;

namespace PostsByMarko.Host.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmailHelper emailHelper;
        private readonly IUsersService usersService;
        private readonly ICurrentRequestAccessor currentRequestAccessor;
        private readonly LinkGenerator linkGenerator;

        public EmailService(IEmailHelper emailHelper, IUsersService usersService, LinkGenerator linkGenerator, ICurrentRequestAccessor currentRequestAccessor)
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
            var confirmationLink = linkGenerator.GetUriByAction(currentRequestAccessor.requestContext, action: "ConfirmEmail", controller: "Auth", values: new { token, email = user.Email });
            var subject = $"Please confirm the registration for {user.Email}";
            var body = $"Your account has been successfully created. Please click on the following link to confirm your registration: {confirmationLink}";

            await emailHelper.SendEmailAsync(user.FirstName!, user.LastName!, user.Email, subject, body);
        }

        public async Task ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto)
        {
            var user = await usersService.GetUserByEmailAsync(confirmEmailDto.Email) ?? throw new AuthException($"No account for '{confirmEmailDto.Email}', please check your credentials and try again");
            var emailConfirmed = await usersService.ConfirmEmailForUserAsync(user, confirmEmailDto.Token);

            if (!emailConfirmed.Succeeded)
            {
                throw new AuthException("Error during email confirmation");
            }
        }
    }
}
