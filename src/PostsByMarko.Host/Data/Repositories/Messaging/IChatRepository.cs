using PostsByMarko.Host.Data.Entities;

namespace PostsByMarko.Host.Data.Repositories.Messaging
{
    public interface IChatRepository
    {
        Task<Chat?> GetChatByIdAsync(Guid Id, CancellationToken cancellationToken);
        Task<Chat?> GetChatByUserIdsAsync(Guid[] Ids, CancellationToken cancellationToken);
        Task<List<Chat>> GetChatsForUserAsync(User user, CancellationToken cancellationToken);
        Task<Chat> AddChatAsync(Chat chat, CancellationToken cancellationToken);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
