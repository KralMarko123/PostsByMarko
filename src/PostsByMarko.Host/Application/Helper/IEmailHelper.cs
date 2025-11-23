namespace PostsByMarko.Host.Application.Helper
{
    public interface IEmailHelper
    {
        Task SendEmailAsync(string firstName, string lastName, string emailToSendTo, string subject, string body);
    }
}
