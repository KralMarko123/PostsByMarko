using PostsByMarko.Host.Data.Entities;

using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Requests;
using PostsByMarko.Host.Application.Responses;

namespace PostsByMarko.Host.Data.Repositories.Messaging
{
    public interface IMessageRepository
    {
        Task<PagedResult<MessageDto>> GetMessagesAsync(Guid chatId, PageRequest page, CancellationToken cancellationToken = default);
        Task<Message> AddMessageAsync(Message message, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
