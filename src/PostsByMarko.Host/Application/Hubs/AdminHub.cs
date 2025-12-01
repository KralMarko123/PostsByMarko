using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PostsByMarko.Host.Application.Hubs.Client;

namespace PostsByMarko.Host.Application.Hubs
{
    [Authorize(Roles = "Admin")]
    public class AdminHub : Hub<IAdminClient>
    {
        // Later we can override OnConnectedAsync and join user groups if needed
    }
}
