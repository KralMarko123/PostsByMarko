using PostsByMarko.Host.Application.DTOs;

namespace PostsByMarko.Host.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailConfimationLinkAsync(string emailToSendTo);    
        Task ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto);
    }
}
