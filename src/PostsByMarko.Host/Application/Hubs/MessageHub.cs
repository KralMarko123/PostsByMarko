using Microsoft.AspNetCore.SignalR;
using PostsByMarko.Host.Application.Hubs.Client;

namespace PostsByMarko.Host.Application.Hubs
{
    public class MessageHub : Hub<IMessageClient>
    {
        public async Task NotifyUsersAboutNewMessage(string[] userIds)
        {
            await Clients.Users(userIds).ReceiveMessage($"New message for users: '{string.Join(", ", userIds)}' at {DateTime.UtcNow}");
        }
    }
}
