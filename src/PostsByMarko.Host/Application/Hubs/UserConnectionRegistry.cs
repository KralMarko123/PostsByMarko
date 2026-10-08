using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace PostsByMarko.Host.Application.Hubs;

public interface IUserConnectionRegistry
{
    void Register(Guid userId, HubCallerContext connection);
    void Unregister(string connectionId);
    void Revoke(Guid userId);
}

// Connections belong to this API process. Multiple API instances need shared revocation.
public sealed class UserConnectionRegistry : IUserConnectionRegistry
{
    private readonly ConcurrentDictionary<string, (Guid UserId, HubCallerContext Context)> connections = new();

    public void Register(Guid userId, HubCallerContext connection) =>
        connections[connection.ConnectionId] = (userId, connection);

    public void Unregister(string connectionId) => connections.TryRemove(connectionId, out _);

    public void Revoke(Guid userId)
    {
        foreach (var connection in connections.Values)
        {
            if (connection.UserId == userId)
            {
                connection.Context.Abort();
            }
        }
    }
}
