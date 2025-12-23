using FluentAssertions;
using Microsoft.AspNetCore.Identity;
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

namespace PostsByMarko.IntegrationTests.Controllers
{
    [Collection("IntegrationCollection")]
    public class AuthControllerTests
    {
        private readonly PostsByMarkoApiFactory postsByMarkoApiFactory;
        private readonly HttpClient client;
        private readonly string controllerPrefix = "api/auth";

        public AuthControllerTests(PostsByMarkoApiFactory postsByMarkoApiFactory)
        {
            this.postsByMarkoApiFactory = postsByMarkoApiFactory;
            
            client = postsByMarkoApiFactory.client!;
        }

        [Fact]
        public async Task should_register()
        {
            // Arrange
            var registrationDto = new RegistrationDto { Email = "some_user@somedomain.com", Password = "@SomePassword123" };

            // Act
            var response = await client.PostAsJsonAsync($"{controllerPrefix}/register", registrationDto);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.Should().Be("Successfully registered, please check your email and confirm your account before logging in");
        }

        [Fact]
        public async Task should_login()
        {
            // Arrange
            var testAdmin = await postsByMarkoApiFactory.GetUserByEmailAsync(TestingConstants.TEST_ADMIN_EMAIL);
            var loginDto = new LoginDto { Email = testAdmin.Email, Password = TestingConstants.TEST_PASSWORD };

            // Act
            var response = await client.PostAsJsonAsync($"{controllerPrefix}/login", loginDto);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<LoginResponse>(content);
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            result!.Token.Should().NotBeNullOrEmpty();
            result.Id.Should().Be(testAdmin.Id);
            result.Email.Should().Be(testAdmin.Email);
            result.FirstName.Should().Be(testAdmin.FirstName);
            result.LastName.Should().Be(testAdmin.LastName);
            result.Roles.Should().Contain("Admin");
        }

        [Fact]
        public async Task should_confirm_email()
        {
            // Arrange
            var userManager = postsByMarkoApiFactory.Resolve<UserManager<User>>();
            var unconfirmedUser = await postsByMarkoApiFactory.GetUserByEmailAsync(TestingConstants.UNCONFIRMED_USER_EMAIL);
            var token = await userManager.GenerateEmailConfirmationTokenAsync(unconfirmedUser);

            token = WebUtility.UrlEncode(token);

            // Act
            var response = await client.GetAsync($"{controllerPrefix}/confirm?email={unconfirmedUser.Email}&token={token}");
            var redirect = response.Headers.Location!.ToString();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            redirect.Should().Be("http://localhost:3000/login");
        }
    }
}
