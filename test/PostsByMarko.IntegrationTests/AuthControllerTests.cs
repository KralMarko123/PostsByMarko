using FluentAssertions;
using Newtonsoft.Json;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Host.Data.Models.Dtos;
using PostsByMarko.Host.Data.Models.Responses;
using PostsByMarko.Shared.Constants;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace PostsByMarko.IntegrationTests
{
    [Collection("Integration Collection")]
    public class AuthControllerTests
    {
        private readonly HttpClient client;
        private readonly User testUser = TestingConstants.TEST_USER;

        public AuthControllerTests(PostsByMarkoApiFactory postsByMarkoApiFactory)
        {
            client = postsByMarkoApiFactory.client!;
        }

        [Fact]
        public async Task should_register()
        {
            // Arrange
            var registrationDto = new UserRegistrationDto { Email = "some_user@somedomain.com", Password = "@SomePassword123" };

            // Act
            var response = await client.PostAsJsonAsync("/register", registrationDto);
            var requestResult = await response.Content.ReadFromJsonAsync<RequestResult>();

            // Assert
            requestResult!.StatusCode.Should().Be(HttpStatusCode.Created);
            requestResult!.Message.Should().Be("Successfully Registered");
        }

        [Fact]
        public async Task should_login()
        {
            // Arrange
            var loginDto = new UserLoginDto { Email = testUser.Email, Password = TestingConstants.TEST_PASSWORD };

            // Act
            var response = await client.PostAsJsonAsync("/login", loginDto);
            var requestResult = await response.Content.ReadFromJsonAsync<RequestResult>();
            var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(requestResult.Payload.ToString()!);

            // Assert
            requestResult.Should().NotBeNull();
            requestResult.StatusCode.Should().Be(HttpStatusCode.OK);
            requestResult.Message.Should().Be("Successfully Logged In");

            loginResponse.Should().NotBeNull();
            loginResponse.Email.Should().Be(testUser.Email);
            loginResponse.Token.Should().NotBeNullOrEmpty();
            loginResponse.Roles.Should().Contain("User");
        }
    }
}
