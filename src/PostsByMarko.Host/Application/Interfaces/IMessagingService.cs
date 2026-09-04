using PostsByMarko.Host.Application.DTOs;

using PostsByMarko.Host.Application.Requests;

namespace PostsByMarko.Host.Application.Interfaces
{
    public interface IMessagingService
    {
        Task<List<ChatDto>> GetUserChatsAsync(CancellationToken cancellationToken = default);
        Task<ChatDto> StartChatAsync(Guid otherUserId, CancellationToken cancellationToken = default);
        Task<MessageDto> SendMessageAsync(SendMessageRequest request, CancellationToken cancellationToken = default);
    }
}
