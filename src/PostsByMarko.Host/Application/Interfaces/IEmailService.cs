namespace PostsByMarko.Host.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailConfirmationLinkAsync(string emailToSendTo, CancellationToken cancellationToken = default);
        Task ConfirmEmailAsync(string email, string token);
    }
}
