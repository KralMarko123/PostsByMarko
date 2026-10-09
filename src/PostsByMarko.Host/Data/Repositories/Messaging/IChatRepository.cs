using PostsByMarko.Host.Data.Entities;

using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Requests;
using PostsByMarko.Host.Application.Responses;

namespace PostsByMarko.Host.Data.Repositories.Messaging
{
    public interface IChatRepository
    {
        Task<Chat?> GetChatByIdAsync(Guid Id, CancellationToken cancellationToken);
        Task<Chat?> GetChatByUserIdsAsync(Guid[] Ids, CancellationToken cancellationToken);
        Task<PagedResult<ChatDto>> GetChatsForUserAsync(Guid userId, PageRequest page, CancellationToken cancellationToken);
        Task<ChatDto?> GetChatDetailsAsync(Guid chatId, CancellationToken cancellationToken);
        Task<Chat> GetOrCreateChatAsync(Chat chat, CancellationToken cancellationToken);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
