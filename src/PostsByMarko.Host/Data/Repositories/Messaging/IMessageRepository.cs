using PostsByMarko.Host.Data.Entities;

namespace PostsByMarko.Host.Data.Repositories.Messaging
{
    public interface IMessageRepository
    {
        Task<Message?> GetMessageByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<Message>> GetMessagesByChatIdAsync(Guid chatId, CancellationToken cancellationToken);
        Task<Message> AddMessageAsync(Message message, CancellationToken cancellationToken);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
