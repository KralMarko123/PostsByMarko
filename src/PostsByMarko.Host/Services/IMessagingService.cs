using PostsByMarko.Host.Data.Models;
using PostsByMarko.Host.Data.Models.Dtos;

namespace PostsByMarko.Host.Services
{
    public interface IMessagingService
    {
        Task<RequestResult> StartChatAsync(string[] participantIds);
        Task<RequestResult> SendMessageAsync(MessageDto messageDto);
        Task<RequestResult> GetUserChatsAsync(string userId);
    }
}
