using FluentAssertions;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Host.Data.Models.Dtos;
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
        public AuthControllerTests(BaseFixture baseFixture)
        {
            client = baseFixture.client;
        }

        [Fact]
        public async Task should_register_and_then_try_to_login()
        {
            var registration = new UserRegistrationDto { Email = "some_user@somedomain.com", Password = "@SomePassword123" };
            var login = new UserLoginDto { Email = registration.Email, Password = registration.Password };

            await client.PostAsJsonAsync("/register", registration);

            var response = await client.PostAsJsonAsync("/login", login);
            var result = await response.Content.ReadFromJsonAsync<RequestResult>();

            result!.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            result!.Message.Should().Be("Please check your email and confirm your account before logging in");
        }
    }
}
