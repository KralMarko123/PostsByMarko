using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Host.Data.Models.Dtos;
using PostsByMarko.Host.Data.Models.Responses;
using PostsByMarko.Shared.Constants;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace PostsByMarko.IntegrationTests
{
    public class PostsByMarkoApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
    {
        public HttpClient? client;

        public User testUser = TestingConstants.TEST_USER;

        public async Task InitializeAsync()
        {
            client = CreateClient();
            await ConfigureClientAuthentication();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
            builder.UseEnvironment("Test");
        }

        public new Task DisposeAsync()
        {
            client!.Dispose();

            return Task.CompletedTask;
        }

        private async Task ConfigureClientAuthentication()
        {
            var result = await client!.PostAsJsonAsync("/login", new UserLoginDto { Email = TestingConstants.TEST_USER.Email, Password = TestingConstants.TEST_PASSWORD });
            var typedResult = await result.Content.ReadFromJsonAsync<RequestResult>();
            var payload = JsonConvert.DeserializeObject<LoginResponse>(typedResult!.Payload!.ToString()!);
            var token = payload!.Token;

            client!.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }
    }
}
