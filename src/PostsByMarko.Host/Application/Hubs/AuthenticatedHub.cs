using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PostsByMarko.Host.Data;

namespace PostsByMarko.Host.Application.Hubs;

public abstract class AuthenticatedHub<TClient>(
    IUserConnectionRegistry connections, AppDbContext db) : Hub<TClient> where TClient : class
{
    public override async Task OnConnectedAsync()
    {
        var stamp = Context.User?.FindFirstValue("AspNet.Identity.SecurityStamp");

        if (!Guid.TryParse(Context.User?.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ||
            string.IsNullOrWhiteSpace(stamp))
        {
            Context.Abort();
            return;
        }

        // Register before rechecking the database: revocation either aborts this connection,
        // or this fresh read observes the committed stamp change/deletion.
        connections.Register(userId, Context);

        try
        {
            var currentStamp = await db.Users.AsNoTracking()
                .Where(user => user.Id == userId)
                .Select(user => user.SecurityStamp)
                .SingleOrDefaultAsync(Context.ConnectionAborted);

            if (!string.Equals(stamp, currentStamp, StringComparison.Ordinal))
            {
                connections.Unregister(Context.ConnectionId);
                Context.Abort();
                return;
            }

            await base.OnConnectedAsync();
        }
        catch
        {
            connections.Unregister(Context.ConnectionId);
            Context.Abort();
            throw;
        }
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        connections.Unregister(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}
