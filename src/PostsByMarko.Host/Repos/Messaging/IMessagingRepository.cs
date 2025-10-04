using PostsByMarko.Host.Data.Models;

namespace PostsByMarko.Host.Repos.Messaging
{
    public interface IMessagingRepository
    {
        Task<Chat> GetChatByIdAsync(int id);
        Task<Chat> GetChatByParticipantIdsAsync(string[] ids);
        Task<Chat> CreateChatAsync(Chat chat);
        Task<Message> CreateMessageAsync(Message message);
        Task<bool> DeleteMessageAsync(Message message);
    }
}
