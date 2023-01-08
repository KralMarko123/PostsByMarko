namespace aspnetserver.Helper
{
    public interface IEmailHelper
    {
        Task SendEmail(string firstName, string lastName, string emailToSendTo, string subject, string body);
    }
}
