using Microsoft.AspNetCore.SignalR;
using PostsByMarko.Host.Hubs.Client;

namespace PostsByMarko.Host.Hubs
{
    public class MessageHub : Hub<IMessageClient>
    {
        public async Task SendMessageToUsers(string[] userIds)
        {
            await Clients.Users(userIds).ReceiveMessage($"Message has been sent to users: {string.Join(", ", userIds)}");
        }
    }
}
