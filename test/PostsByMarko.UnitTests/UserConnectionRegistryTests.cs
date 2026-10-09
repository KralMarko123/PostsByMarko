using Microsoft.AspNetCore.SignalR;
using Moq;
using PostsByMarko.Host.Application.Hubs;

namespace PostsByMarko.UnitTests;

public class UserConnectionRegistryTests
{
    [Fact]
    public void revocation_aborts_every_connection_for_the_user_and_leaves_other_users_connected()
    {
        // Arrange
        var registry = new UserConnectionRegistry();
        var target = Guid.NewGuid();
        var first = Connection("post");
        var second = Connection("admin");
        var other = Connection("other-user");
        registry.Register(target, first.Object);
        registry.Register(target, second.Object);
        registry.Register(Guid.NewGuid(), other.Object);

        // Act
        registry.Revoke(target);

        // Assert
        first.Verify(connection => connection.Abort(), Times.Once);
        second.Verify(connection => connection.Abort(), Times.Once);
        other.Verify(connection => connection.Abort(), Times.Never);
    }

    [Fact]
    public void disconnected_connections_are_removed()
    {
        // Arrange
        var registry = new UserConnectionRegistry();
        var userId = Guid.NewGuid();
        var connection = Connection("closed");
        registry.Register(userId, connection.Object);
        // Act
        registry.Unregister(connection.Object.ConnectionId);

        registry.Revoke(userId);

        // Assert
        connection.Verify(context => context.Abort(), Times.Never);
    }

    private static Mock<HubCallerContext> Connection(string id)
    {
        var connection = new Mock<HubCallerContext>();
        connection.Setup(context => context.ConnectionId).Returns(id);
        return connection;
    }
}
