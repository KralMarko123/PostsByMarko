using Newtonsoft.Json;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Shared.Constants;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace PostsByMarko.IntegrationTests
{
    [Collection("Integration Collection")]
    public class MessageControllerTests
    {
        private readonly HttpClient client;
        private readonly User testUser = TestingConstants.TEST_USER;
        private readonly User randomUser = TestingConstants.RANDOM_USER;

        public MessageControllerTests(PostsByMarkoApiFactory postsByMarkoApiFactory)
        {
            client = postsByMarkoApiFactory.client!;
        }

        [Fact]
        public async Task should_create_chat()
        {
            // Arrange
            var participantIds = [testUser.Id, randomUser.Id];

            // Act
            var response = await client.GetFromJsonAsync<RequestResult>("/getAllUsers");
            var users = JsonConvert.DeserializeObject<List<User>>(response!.Payload!.ToString()!);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            users.Should().NotBeNullOrEmpty();
            users.Select(u => u.Email).Should().Contain(testUser.Email);
        }
    }
}
