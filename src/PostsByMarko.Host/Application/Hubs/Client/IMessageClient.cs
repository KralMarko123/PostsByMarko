using PostsByMarko.Host.Application.DTOs;

namespace PostsByMarko.Host.Application.Hubs.Client
{
    public interface IMessageClient
    {
        Task MessageSent(MessageDto message);
        Task ChatCreated(ChatDto chat);
    }
}
