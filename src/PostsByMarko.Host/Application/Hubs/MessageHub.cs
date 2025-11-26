using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PostsByMarko.Host.Application.Hubs.Client;

namespace PostsByMarko.Host.Application.Hubs
{
    [Authorize]
    public class MessageHub : Hub<IMessageClient>
    {
        // Later we can override OnConnectedAsync and join user groups if needed
    }
}
