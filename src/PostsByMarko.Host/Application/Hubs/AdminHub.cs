using Microsoft.AspNetCore.Authorization;
using PostsByMarko.Host.Application.Hubs.Client;
using PostsByMarko.Host.Data;

namespace PostsByMarko.Host.Application.Hubs
{
    [Authorize(Roles = "Admin")]
    public class AdminHub(IUserConnectionRegistry connections, AppDbContext db)
        : AuthenticatedHub<IAdminClient>(connections, db) { }
}
