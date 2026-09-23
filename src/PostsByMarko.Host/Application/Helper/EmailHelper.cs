using Microsoft.Extensions.Options;
using MimeKit;
using PostsByMarko.Host.Application.Configuration;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace PostsByMarko.Host.Application.Helper
{
    public class EmailHelper : IEmailHelper
    {
        private readonly EmailConfig emailConfig;
        private readonly ILogger<EmailHelper> logger;

        public EmailHelper(IOptions<EmailConfig> emailConfig, ILogger<EmailHelper> logger)
        {
            this.emailConfig = emailConfig.Value;
            this.logger = logger;
        }

        public async Task SendEmailAsync(string firstName, string lastName, string emailToSendTo, string subject, string body, CancellationToken cancellationToken = default)
        {
            if (!emailConfig.Enabled)
            {
                logger.LogInformation("Email delivery is disabled. Skipping message to {Recipient}.", emailToSendTo);
                return;
            }

            var message = new MimeMessage();

            var senderAddress = string.IsNullOrWhiteSpace(emailConfig.SenderAddress)
                ? emailConfig.Username : emailConfig.SenderAddress;
            message.From.Add(new MailboxAddress(emailConfig.SenderName, senderAddress));
            message.To.Add(new MailboxAddress($"{firstName} {lastName}", emailToSendTo));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            try
            {
                using var client = new SmtpClient();
                await client.ConnectAsync(emailConfig.Host, emailConfig.Port, emailConfig.UseSsl, cancellationToken);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                if (!string.IsNullOrWhiteSpace(emailConfig.Username))
                {
                    await client.AuthenticateAsync(emailConfig.Username, emailConfig.Password, cancellationToken);
                }

                await client.SendAsync(message, cancellationToken);
                await client.DisconnectAsync(true, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send email to {Recipient}.", emailToSendTo);
                throw;
            }
        }
    }
}
