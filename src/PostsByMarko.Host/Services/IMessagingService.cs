using PostsByMarko.Host.Data.Models;

namespace PostsByMarko.Host.Services
{
    public interface IMessagingService
    {
        Task<RequestResult> StartChatAsync(string[] participantIds);
        Task<RequestResult> SendMessageAsync(int chatId, string senderId, string content);
    }
}
