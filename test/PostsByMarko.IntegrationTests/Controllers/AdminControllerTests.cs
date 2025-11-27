using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using PostsByMarko.Host.Application.Enums;
using PostsByMarko.Host.Application.Requests;
using PostsByMarko.Host.Application.Responses;
using PostsByMarko.Host.Data.Entities;
using PostsByMarko.Test.Shared.Constants;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace PostsByMarko.IntegrationTests.Controllers
{
    [Collection("IntegrationCollection")]
    public class AdminControllerTests : IAsyncLifetime
    {
        private readonly PostsByMarkoApiFactory postsByMarkoApiFactory;
        private readonly HttpClient client;
        private readonly string controllerPrefix = "/api/admin";

        public AdminControllerTests(PostsByMarkoApiFactory postsByMarkoApiFactory)
        {
            this.postsByMarkoApiFactory = postsByMarkoApiFactory;

            client = postsByMarkoApiFactory.client!;
        }

        public async Task InitializeAsync()
        {
            await postsByMarkoApiFactory.RecreateAndSeedDatabaseAsync();
            await postsByMarkoApiFactory.AuthenticateClientAsync(client);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task should_return_roles_for_email()
        {
            // Arrange
            var expectedRoles = new List<string> { "Admin", "User" };

            // Act
            var response = await client.GetAsync($"{controllerPrefix}/roles?email={TestingConstants.TEST_ADMIN_EMAIL}");
            var responseContent = await response.Content.ReadAsStringAsync();
            var roles = JsonConvert.DeserializeObject<List<string>>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            roles.Should().NotBeNullOrEmpty();
            roles.Should().BeEquivalentTo(expectedRoles);
        }

        [Fact]
        public async Task should_delete_a_user()
        {
            // Arrange
            var userManager = postsByMarkoApiFactory.Resolve<UserManager<User>>();
            var userToDelete = await postsByMarkoApiFactory.GetUserByEmailAsync(TestingConstants.UNCONFIRMED_USER_EMAIL);

            // Act
            var response = await client.DeleteAsync($"{controllerPrefix}/users/{userToDelete.Id}");
            var deletedUser = await userManager.FindByIdAsync(userToDelete.Id.ToString());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            deletedUser.Should().BeNull();
        }

        [Fact]
        public async Task should_update_user_roles()
        {
            // Arrange
            var userManager = postsByMarkoApiFactory.Resolve<UserManager<User>>();
            var user = await postsByMarkoApiFactory.GetUserByEmailAsync(TestingConstants.TEST_USER_EMAIL);
            var rolesBeforeUpdate = await userManager.GetRolesAsync(user);
            var updateRolesRequest = new UpdateUserRolesRequest
            {
                UserId = user.Id,
                ActionType = ActionType.Create,
                Role = "Admin"
            };

            // Act
            var response = await client.PutAsJsonAsync($"{controllerPrefix}/roles", updateRolesRequest);
            var responseContent = await response.Content.ReadAsStringAsync();
            var roles = JsonConvert.DeserializeObject<List<string>>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            roles.Should().NotBeNullOrEmpty();
            roles.Count.Should().Be(rolesBeforeUpdate.Count + 1);
            roles.Should().Contain("Admin");
        }

        [Fact]
        public async Task should_get_admin_dashboard_data()
        {
            // Arrange
           
            // Act
            var response = await client.GetAsync($"{controllerPrefix}/dashboard");
            var responseContent = await response.Content.ReadAsStringAsync();
            var dashboardData = JsonConvert.DeserializeObject<List<AdminDashboardResponse>>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            dashboardData.Should().NotBeNullOrEmpty();
            dashboardData.Should().AllBeOfType<AdminDashboardResponse>();
        }
    }
}
