﻿using MimeKit;
using PostsByMarko.Host.Constants;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace PostsByMarko.Host.Helper
{
    public class EmailHelper : IEmailHelper
    {
        public async Task SendEmail(string firstName, string lastName, string emailToSendTo, string subject, string body)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("ADMIN @ Posts By Marko", MiscConstants.SERVER_EMAIL));
            message.To.Add(new MailboxAddress($"{firstName} {lastName}", emailToSendTo));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            try
            {
                using var client = new SmtpClient();
                client.Connect("mail.markomarkovikj.com", 465, true);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(MiscConstants.SERVER_EMAIL, "@Ilovedrums123");

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
