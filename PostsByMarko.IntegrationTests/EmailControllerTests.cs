using FluentAssertions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace PostsByMarko.IntegrationTests
{
    [Collection("Integration Collection")]
    public class EmailControllerTests
    {
        private readonly HttpClient client;
        public EmailControllerTests(PostsByMarkoApiFactory postsByMarkoApiFactory)
        {
            client = postsByMarkoApiFactory.client;
        }

        [Fact]
        public async Task should_return_not_found_when_confirming_given_a_nonexistent_email()
        {
            // Arrange
            var email = "other_user@test.com";
            var token = "some_token";

            // Act
            var response = await client.GetAsync($"/confirmEmail?token={token}&emaIL={email}");
            var message = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            message.Should().Be($"User with username: {email} was not found");
        }
    }
}
