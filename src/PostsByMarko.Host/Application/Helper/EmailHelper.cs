using MimeKit;
using PostsByMarko.Host.Application.Constants;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace PostsByMarko.Host.Application.Helper
{
    public class EmailHelper : IEmailHelper
    {
        public async Task SendEmailAsync(string firstName, string lastName, string emailToSendTo, string subject, string body)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("ADMIN @ Posts By Marko", MiscConstants.SERVER_EMAIL));
            message.To.Add(new MailboxAddress($"{firstName} {lastName}", emailToSendTo));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            try
            {
                using var client = new SmtpClient();
                await client.ConnectAsync("mail.markomarkovikj.com", 465, true);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                await client.AuthenticateAsync(MiscConstants.SERVER_EMAIL, "@Ilovemotorcycles123");

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
