using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PostsByMarko.Host.Application.Configuration;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Enums;
using PostsByMarko.Host.Application.Requests;
using PostsByMarko.Host.Application.Responses;
using PostsByMarko.Host.Application.Hubs;
using PostsByMarko.Host.Data;
using PostsByMarko.Host.Data.Entities;
using PostsByMarko.Test.Shared.Constants;
using Xunit;

namespace PostsByMarko.IntegrationTests.Hubs;

[Collection("IntegrationCollection")]
public class ConnectionRevocationTests(PostsByMarkoApiFactory factory) : IAsyncLifetime
{
    public Task InitializeAsync() => factory.RecreateAndSeedDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task connection_start_rechecks_stamp_even_when_authentication_cached_the_user(bool delete)
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var user = (await users.FindByEmailAsync(TestingConstants.TEST_USER_EMAIL))!;
        var context = new TestConnectionContext(new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("AspNet.Identity.SecurityStamp", user.SecurityStamp!)
        ], "Bearer")));
        // Simulate a committed change after HTTP authentication but before hub registration.
        using (var mutation = factory.Services.CreateScope())
        {
            var otherUsers = mutation.ServiceProvider.GetRequiredService<UserManager<User>>();
            var otherUser = (await otherUsers.FindByIdAsync(user.Id.ToString()))!;
            var result = delete ? await otherUsers.DeleteAsync(otherUser) : await otherUsers.UpdateSecurityStampAsync(otherUser);
            Assert.True(result.Succeeded);
        }

        var registry = new UserConnectionRegistry();
        using var hub = new PostHub(registry, scope.ServiceProvider.GetRequiredService<AppDbContext>()) { Context = context };
        // Act
        await hub.OnConnectedAsync();

        // Assert
        Assert.Equal(1, context.AbortCount);
        registry.Revoke(user.Id);
        Assert.Equal(1, context.AbortCount); // Rejected connections must also be removed from the registry.
    }

    [Theory]
    [InlineData("adminHub", false)]
    [InlineData("postHub", false)]
    [InlineData("messageHub", false)]
    [InlineData("adminHub", true)]
    [InlineData("postHub", true)]
    [InlineData("messageHub", true)]
    public async Task demotion_or_deletion_closes_existing_websocket_and_rejects_old_token(string hub, bool delete)
    {
        // Arrange
        using var actor = factory.CreateClient();
        var actorToken = await LoginAsync(actor, TestingConstants.OWNER_EMAIL);
        actor.DefaultRequestHeaders.Authorization = new("Bearer", actorToken);
        using var targetClient = factory.CreateClient();
        var targetToken = await LoginAsync(targetClient, TestingConstants.TEST_ADMIN_EMAIL);
        var target = await factory.GetUserByEmailAsync(TestingConstants.TEST_ADMIN_EMAIL);
        await using var targetConnection = Connect(hub, targetToken);
        await using var observer = Connect("adminHub", actorToken);
        var closed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        targetConnection.Closed += _ => { closed.TrySetResult(); return Task.CompletedTask; };
        var notified = new TaskCompletionSource<Guid>(TaskCreationOptions.RunContinuationsAsynchronously);
        observer.On<Guid, DateTime>(delete ? "DeletedUser" : "UpdatedUserRoles", (id, _) => notified.TrySetResult(id));
        await targetConnection.StartAsync();
        await observer.StartAsync();

        // Act
        using var response = delete
            ? await actor.DeleteAsync($"/api/admin/users/{target.Id}")
            : await actor.PutAsJsonAsync("/api/admin/roles", new UpdateUserRolesRequest
            {
                UserId = target.Id,
                Role = "Admin",
                ActionType = ActionType.Delete
            });
        // Assert
        response.EnsureSuccessStatusCode();

        await closed.Task.WaitAsync(TimeSpan.FromSeconds(10));
        Assert.Equal(target.Id, await notified.Task.WaitAsync(TimeSpan.FromSeconds(10)));
        Assert.Equal(HubConnectionState.Disconnected, targetConnection.State);
        Assert.Equal(HubConnectionState.Connected, observer.State);
        await Assert.ThrowsAnyAsync<Exception>(() => targetConnection.StartAsync());
    }

    [Theory]
    [InlineData("adminHub")]
    [InlineData("postHub")]
    [InlineData("messageHub")]
    public async Task jwt_expiration_closes_existing_websocket(string hub)
    {
        // Arrange
        using var client = factory.CreateClient();
        var token = await LoginAsync(client, TestingConstants.TEST_ADMIN_EMAIL);
        using var scope = factory.Services.CreateScope();
        var config = scope.ServiceProvider.GetRequiredService<IOptions<JwtConfig>>().Value;
        var handler = new JwtSecurityTokenHandler();
        var original = handler.ReadJwtToken(token);
        original.Payload["exp"] = DateTimeOffset.UtcNow.AddSeconds(4).ToUnixTimeSeconds();
        var shortToken = handler.WriteToken(new JwtSecurityToken(
            new JwtHeader(new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Secret)),
                SecurityAlgorithms.HmacSha256)), original.Payload));
        await using var connection = Connect(hub, shortToken);
        var closed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        connection.Closed += _ => { closed.TrySetResult(); return Task.CompletedTask; };

        // Act
        await connection.StartAsync();
        await closed.Task.WaitAsync(TimeSpan.FromSeconds(10));

        // Assert
        Assert.Equal(HubConnectionState.Disconnected, connection.State);
    }

    private HubConnection Connect(string hub, string token) => new HubConnectionBuilder()
        .WithUrl($"{factory.Server.BaseAddress}{hub}", options =>
        {
            options.Transports = HttpTransportType.WebSockets;
            options.SkipNegotiation = true;
            options.AccessTokenProvider = () => Task.FromResult<string?>(token);
            options.WebSocketFactory = async (context, cancellationToken) =>
            {
                var socket = factory.Server.CreateWebSocketClient();
                socket.ConfigureRequest = request => request.Headers.Authorization = $"Bearer {token}";
                return await socket.ConnectAsync(context.Uri, cancellationToken);
            };
        }).Build();

    private static async Task<string> LoginAsync(HttpClient client, string email)
    {
        using var response = await client.PostAsJsonAsync("/api/auth/login",
            new LoginDto { Email = email, Password = TestingConstants.TEST_PASSWORD });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<LoginResponse>())!.Token!;
    }

    private sealed class TestConnectionContext(ClaimsPrincipal principal) : HubCallerContext
    {
        public int AbortCount { get; private set; }
        public override string ConnectionId => "connection-start-race";
        public override string? UserIdentifier => principal.FindFirstValue(ClaimTypes.NameIdentifier);
        public override ClaimsPrincipal User => principal;
        public override IDictionary<object, object?> Items { get; } = new Dictionary<object, object?>();
        public override IFeatureCollection Features { get; } = new FeatureCollection();
        public override CancellationToken ConnectionAborted => CancellationToken.None;
        public override void Abort() => AbortCount++;
    }
}
