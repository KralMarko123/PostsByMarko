using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using PostsByMarko.Host.Application.Constants;
using PostsByMarko.Host.Application.Enums;
using PostsByMarko.Host.Application.Requests;
using PostsByMarko.Host.Application.Responses;
using PostsByMarko.Host.Data.Entities;
using PostsByMarko.Host.Data.Repositories.Users;
using PostsByMarko.Test.Shared.Constants;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
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

        [Fact]
        public async Task should_reject_deleting_current_admin()
        {
            var currentAdmin = await postsByMarkoApiFactory.GetUserByEmailAsync(TestingConstants.TEST_ADMIN_EMAIL);

            var response = await client.DeleteAsync($"{controllerPrefix}/users/{currentAdmin.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            (await postsByMarkoApiFactory.GetUserByEmailAsync(TestingConstants.TEST_ADMIN_EMAIL))
                .Should().NotBeNull();
        }

        [Fact]
        public async Task should_reject_unknown_role()
        {
            var user = await postsByMarkoApiFactory.GetUserByEmailAsync(TestingConstants.TEST_USER_EMAIL);
            var request = new UpdateUserRolesRequest
            {
                UserId = user.Id,
                ActionType = ActionType.Create,
                Role = "SuperAdmin"
            };

            var response = await client.PutAsJsonAsync($"{controllerPrefix}/roles", request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task repository_should_preserve_last_admin()
        {
            var repository = postsByMarkoApiFactory.Resolve<IUserRepository>();
            var owner = await postsByMarkoApiFactory.GetUserByEmailAsync(TestingConstants.OWNER_EMAIL);
            var testAdmin = await postsByMarkoApiFactory.GetUserByEmailAsync(TestingConstants.TEST_ADMIN_EMAIL);
            (await repository.RemoveRoleFromUserAsync(owner, RoleConstants.ADMIN)).Succeeded.Should().BeTrue();

            var result = await repository.RemoveRoleFromUserUnlessLastMemberAsync(
                testAdmin, RoleConstants.ADMIN, CancellationToken.None);

            result.Succeeded.Should().BeFalse();
            result.Errors.Should().ContainSingle(error => error.Code == IdentityErrorCodes.LastMemberInRole);
        }
    }
}
