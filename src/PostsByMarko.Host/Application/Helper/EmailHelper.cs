using Microsoft.Extensions.Options;
using MimeKit;
using PostsByMarko.Host.Application.Configuration;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace PostsByMarko.Host.Application.Helper
{
    public class EmailHelper : IEmailHelper
    {
        private readonly EmailConfig emailConfig;

        public EmailHelper(IOptions<EmailConfig> emailConfig)
        {
            this.emailConfig = emailConfig.Value;
        }

        public async Task SendEmailAsync(string firstName, string lastName, string emailToSendTo, string subject, string body)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(emailConfig.SenderName, emailConfig.Username));
            message.To.Add(new MailboxAddress($"{firstName} {lastName}", emailToSendTo));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            try
            {
                using var client = new SmtpClient();
                await client.ConnectAsync(emailConfig.Host, emailConfig.Port, emailConfig.UseSsl);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                await client.AuthenticateAsync(emailConfig.Username, emailConfig.Password);

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
