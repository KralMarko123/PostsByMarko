using PostsByMarko.Host.Data.Entities;

namespace PostsByMarko.Host.Data.Repositories.Messaging
{
    public interface IMessageRepository
    {
        Task<Message?> GetMessageByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Message>> GetMessagesByChatIdAsync(Guid chatId, CancellationToken cancellationToken = default);
        Task<Message> AddMessageAsync(Message message, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
