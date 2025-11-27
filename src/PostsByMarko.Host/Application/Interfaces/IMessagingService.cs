using PostsByMarko.Host.Application.DTOs;

namespace PostsByMarko.Host.Application.Interfaces
{
    public interface IMessagingService
    {
        Task<List<ChatDto>> GetUserChatsAsync(CancellationToken cancellationToken = default);
        Task<ChatDto> StartChatAsync(Guid otherUserId, CancellationToken cancellationToken = default);
        Task<MessageDto> SendMessageAsync(MessageDto messageDto, CancellationToken cancellationToken = default);
    }
}
