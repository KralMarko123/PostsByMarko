using PostsByMarko.Host.Application.DTOs;

using PostsByMarko.Host.Application.Requests;
using PostsByMarko.Host.Application.Responses;

namespace PostsByMarko.Host.Application.Interfaces
{
    public interface IMessagingService
    {
        Task<PagedResult<ChatDto>> GetUserChatsAsync(PageRequest page, CancellationToken cancellationToken = default);
        Task<PagedResult<MessageDto>> GetChatMessagesAsync(Guid chatId, PageRequest page, CancellationToken cancellationToken = default);
        Task<ChatDto> StartChatAsync(Guid otherUserId, CancellationToken cancellationToken = default);
        Task<MessageDto> SendMessageAsync(SendMessageRequest request, CancellationToken cancellationToken = default);
    }
}
