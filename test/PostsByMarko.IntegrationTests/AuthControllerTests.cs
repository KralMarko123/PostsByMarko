using FluentAssertions;
using Newtonsoft.Json;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Responses;
using PostsByMarko.Host.Data.Entities;
using PostsByMarko.Test.Shared.Constants;
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
        private readonly User testAdmin = TestingConstants.TEST_ADMIN;

        public AuthControllerTests(PostsByMarkoApiFactory postsByMarkoApiFactory)
        {
            client = postsByMarkoApiFactory.client!;
        }

        [Fact]
        public async Task should_register()
        {
            // Arrange
            var registrationDto = new RegistrationDto { Email = "some_user@somedomain.com", Password = "@SomePassword123" };

            // Act
            var response = await client.PostAsJsonAsync("api/auth/register", registrationDto);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.Should().Be("Successfully registered, please check your email and confirm your account before logging in");
        }

        [Fact]
        public async Task should_login()
        {
            // Arrange
            var loginDto = new LoginDto { Email = testAdmin.Email, Password = TestingConstants.TEST_PASSWORD };

            // Act
            var response = await client.PostAsJsonAsync("/login", loginDto);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<LoginResponse>(content);
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            result!.Token.Should().NotBeNullOrEmpty();
            result.Id.Should().Be(testAdmin.Id);
            result.Email.Should().Be(testAdmin.Email);
            result.FirstName.Should().Be(testAdmin.FirstName);
            result.LastName.Should().Be(testAdmin.LastName);
        }
    }
}
