using FluentAssertions;
using Microsoft.AspNetCore.SignalR.Client;
using PostsByMarko.Host.Application.Enums;
using PostsByMarko.Host.Application.Requests;
using PostsByMarko.Test.Shared.Constants;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace PostsByMarko.IntegrationTests.Hubs
{
    [Collection("IntegrationCollection")]
    public class AdminHubTests : IAsyncLifetime
    {
        private readonly PostsByMarkoApiFactory postsByMarkoApiFactory;
        private readonly HttpClient client;
        private HubConnection? hubConnection;
        private readonly string controllerPrefix = "/api/admin";

        public AdminHubTests(PostsByMarkoApiFactory postsByMarkoApiFactory)
        {
            this.postsByMarkoApiFactory = postsByMarkoApiFactory;

            client = postsByMarkoApiFactory.client!;
        }

        public async Task InitializeAsync()
        {
            await postsByMarkoApiFactory.RecreateAndSeedDatabaseAsync();
            await postsByMarkoApiFactory.AuthenticateClientAsync(client);

            hubConnection = postsByMarkoApiFactory.CreateHubConnection("adminHub");

            await hubConnection.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await hubConnection!.StopAsync();
            await hubConnection.DisposeAsync();
        }

        [Fact]
        public async Task updating_user_roles_notifies_the_admins()
        {
            // Arrange
            var testAdmin = await postsByMarkoApiFactory.GetUserByEmailAsync(TestingConstants.TEST_ADMIN_EMAIL);
            var testUser = await postsByMarkoApiFactory.GetUserByEmailAsync(TestingConstants.TEST_USER_EMAIL);
            var updateRequest = new UpdateUserRolesRequest
            {
                UserId = testUser.Id,
                ActionType = ActionType.Create,
                Role = "Admin"
            };

            Guid? userId = null;
            DateTime? updatedAt = null;
            hubConnection!.On<Guid, DateTime>("UpdatedUserRoles", (id, timestamp) =>
            {
                userId = id;
                updatedAt = timestamp;
            });

            // Act
            await client.PutAsJsonAsync($"{controllerPrefix}/roles", updateRequest);
            await postsByMarkoApiFactory.WaitForSignalRPropagation();

            // Assert
            userId.Should().Be(testUser.Id);
            updatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task deleting_a_user_notifies_the_admins()
        {
            // Arrange
            var testAdmin = await postsByMarkoApiFactory.GetUserByEmailAsync(TestingConstants.TEST_ADMIN_EMAIL);
            var testUser = await postsByMarkoApiFactory.GetUserByEmailAsync(TestingConstants.TEST_USER_EMAIL);

            Guid? userId = null;
            DateTime? deletedAt = null;
            hubConnection!.On<Guid, DateTime>("DeletedUser", (id, timestamp) =>
            {
                userId = id;
                deletedAt = timestamp;
            });

            // Act
            await client.DeleteAsync($"{controllerPrefix}/users/{testUser.Id}");
            await postsByMarkoApiFactory.WaitForSignalRPropagation();

            // Assert
            userId.Should().Be(testUser.Id);
            deletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }
    }
}
