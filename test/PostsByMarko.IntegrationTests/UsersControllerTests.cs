using FluentAssertions;
using Newtonsoft.Json;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Shared.Constants;
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
            users.Select(u => u.Email).Should().Contain(TestingConstants.TEST_USER.Email);
        }
    }
}