namespace PostsByMarko.Host.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailConfimationLinkAsync(string emailToSendTo);    
        Task ConfirmEmailAsync(string email, string token);
    }
}
