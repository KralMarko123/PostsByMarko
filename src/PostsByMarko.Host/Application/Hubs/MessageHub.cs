using Microsoft.AspNetCore.Authorization;
using PostsByMarko.Host.Application.Hubs.Client;
using PostsByMarko.Host.Data;

namespace PostsByMarko.Host.Application.Hubs
{
    [Authorize]
    public class MessageHub(IUserConnectionRegistry connections, AppDbContext db)
        : AuthenticatedHub<IMessageClient>(connections, db) { }
}
