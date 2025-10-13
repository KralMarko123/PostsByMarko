using Bogus;
using FluentAssertions;
using Newtonsoft.Json;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Host.Data.Models.Responses;
using PostsByMarko.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace PostsByMarko.IntegrationTests
{
    [Collection("Integration Collection")]
    public class UsersControllerTests
    {
        private readonly HttpClient client;
        private readonly User testUser = TestingConstants.TEST_USER;
        private readonly User randomUser = TestingConstants.RANDOM_USER;

        public UsersControllerTests(PostsByMarkoApiFactory postsByMarkoApiFactory)
        {
            client = postsByMarkoApiFactory.client!;
        }

        [Fact]
        public async Task should_return_a_list_of_all_users()
        {
            // Arrange

            // Act
            var response = await client.GetFromJsonAsync<RequestResult>("/getAllUsers");
            var users = JsonConvert.DeserializeObject<List<User>>(response!.Payload!.ToString()!);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            users.Should().NotBeNullOrEmpty();
            users.Select(u => u.Email).Should().Contain(testUser.Email);
        }

        [Fact]
        public async Task should_return_a_list_of_other_users()
        {
            // Arrange

            // Act
            var response = await client.GetFromJsonAsync<RequestResult>("/getOtherUsers");
            var users = JsonConvert.DeserializeObject<List<AuthorDetailsResponse>>(response!.Payload!.ToString()!);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            users.Should().NotBeNullOrEmpty();
            users.Should().NotContain(new AuthorDetailsResponse(testUser));
        }

        [Fact]
        public async Task should_return_a_user()
        {
            // Arrange

            // Act
            var response = await client.GetFromJsonAsync<RequestResult>($"/getUser/{testUser.Id}");
            var user = JsonConvert.DeserializeObject<User>(response!.Payload!.ToString()!);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            user.Should().NotBeNull();
            user.Id.Should().Be(testUser.Id);
        }

        [Fact]
        public async Task should_return_not_found_if_a_user_does_not_exist()
        {
            // Arrange
            var randomId = Guid.NewGuid().ToString();

            // Act
            var response = await client.GetFromJsonAsync<RequestResult>($"/getUser/{randomId}");

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Message.Should().Be($"User with Id: {randomId} was not found");
            response.Payload.Should().BeNull();
        }

        [Fact]
        public async Task should_return_list_of_roles_for_a_given_email()
        {
            // Arrange

            // Act
            var response = await client.GetFromJsonAsync<RequestResult>($"/getEmailRoles/{testUser.Email}");
            var roles = JsonConvert.DeserializeObject<List<string>>(response!.Payload!.ToString()!);

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            roles.Should().Contain(["User"]);
        }

        [Fact]
        public async Task should_return_an_appropriate_message_if_no_roles_exist_for_an_email()
        {
            // Arrange
            var randomEmail = "random@random.com";

            // Act
            var response = await client.GetFromJsonAsync<RequestResult>($"/getEmailRoles/{randomEmail}");
            var roles = JsonConvert.DeserializeObject<List<string>>(response!.Payload!.ToString()!);

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Message.Should().Be("Email has no roles associated with it");

            roles.Should().BeEmpty();
        }

        [Fact]
        public async Task should_return_admin_dashboard_data()
        {
            // Arrange

            // Act
            var response = await client.GetFromJsonAsync<RequestResult>($"/getAdminDashboard");
            var data = JsonConvert.DeserializeObject<List<AdminDashboardResponse>>(response!.Payload!.ToString()!);

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            data.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task should_delete_a_user()
        {
            // Arrange

            // Act
            var response = await client.DeleteFromJsonAsync<RequestResult>($"/deleteUser/{randomUser.Id}");

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Message.Should().Be($"User with Id: {randomUser.Id} removed successfully");
        }

        [Fact]
        public async Task should_return_not_found_when_deleting_an_unknown_user()
        {
            // Arrange
            var randomId = Guid.NewGuid().ToString();

            // Act
            var response = await client.DeleteFromJsonAsync<RequestResult>($"/deleteUser/{randomId}");

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Message.Should().Be($"User with Id: {randomId} was not found");
        }

        [Fact]
        public async Task should_add_a_role_to_a_user()
        {
            // Arrange

            // Act
            var response = await client.PostAsync($"/addRoleToUser/{testUser.Id}/Admin", null);
            var requestResult = await response.Content.ReadFromJsonAsync<RequestResult>();

            var rolesResponse = await client.GetFromJsonAsync<RequestResult>($"/getEmailRoles/{testUser.Email}");
            var roles = JsonConvert.DeserializeObject<List<string>>(rolesResponse!.Payload!.ToString()!);

            // Assert
            requestResult.Should().NotBeNull();
            requestResult.StatusCode.Should().Be(HttpStatusCode.OK);
            requestResult.Message.Should().Be("Role successfully added to user");

            roles.Should().Contain("Admin");
        }

        [Fact]
        public async Task should_remove_a_role_from_a_user()
        {
            // Arrange

            // Act
            var response = await client.PostAsync($"/removeRoleFromUser/{testUser.Id}/USER", null);
            var requestResult = await response.Content.ReadFromJsonAsync<RequestResult>();

            var rolesResponse = await client.GetFromJsonAsync<RequestResult>($"/getEmailRoles/{testUser.Email}");
            var roles = JsonConvert.DeserializeObject<List<string>>(rolesResponse!.Payload!.ToString()!);

            // Assert
            requestResult.Should().NotBeNull();
            requestResult.StatusCode.Should().Be(HttpStatusCode.OK);
            requestResult.Message.Should().Be("Role successfully removed from user");

            roles.Should().NotContain("USER");
            roles.Should().BeEmpty();
        }
    }
}