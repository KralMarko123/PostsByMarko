using aspnetserver.Constants;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace aspnetserver.Helper
{
    public class EmailHelper : IEmailHelper
    {
        public async Task SendEmail(string firstName, string lastName, string emailToSendTo, string confirmationLink)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("ADMIN @ Posts By Marko", AppConstants.webserverAdminEmail));
            message.To.Add(new MailboxAddress($"{firstName} {lastName}", emailToSendTo));
            message.Subject = $"Please confirm the registration for {emailToSendTo}";
            message.Body = new TextPart("plain")
            {
                Text = $"Your account has been successfully created. Please click on the following link to confirm your registration: {confirmationLink}"
            };

            try
            {
                using var client = new SmtpClient();
                client.Connect("mail.markomarkovikj.com", 465, true);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(AppConstants.webserverAdminEmail, "nba2k13ftw");

                await client.SendAsync(message);
                client.Disconnect(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
