using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
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

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddEnvironmentVariables();
            });
        }

        public new Task DisposeAsync()
        {
            client!.Dispose();

            return Task.CompletedTask;
        }

        private async Task ConfigureClientAuthentication()
        {
            const int maxRetries = 5;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    var result = await client!.PostAsJsonAsync("/login", new UserLoginDto { Email = testUser.Email, Password = TestingConstants.TEST_PASSWORD });

                    if (result.IsSuccessStatusCode)
                    {
                        var requestResult = await result.Content.ReadFromJsonAsync<RequestResult>();

                        if (requestResult?.Payload != null)
                        {
                            var payload = JsonConvert.DeserializeObject<LoginResponse>(requestResult.Payload.ToString()!);
                            var token = payload!.Token;
                            client!.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                            return;
                        }
                    }
                }
                catch { /* ignore and retry */ }

                await Task.Delay(2000); // wait 2 seconds before next try
            }

            throw new Exception("Failed to authenticate test client after multiple retries.");
        }
    }
}
