using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Responses;
using PostsByMarko.Host.Data.Entities;
using PostsByMarko.Test.Shared.Constants;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace PostsByMarko.IntegrationTests
{
    public class PostsByMarkoApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        public HttpClient? client;

        public User testAdmin = TestingConstants.TEST_ADMIN;

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
            client?.Dispose();

            return Task.CompletedTask;
        }

        private async Task ConfigureClientAuthentication()
        {
            const int maxRetries = 5;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    var result = await client!.PostAsJsonAsync("api/auth/login", new LoginDto { Email = testAdmin.Email, Password = TestingConstants.TEST_PASSWORD });

                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadFromJsonAsync<LoginResponse>();
                        var token = response!.Token;
                        
                        client!.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                        
                        return;
                    }
                }
                catch { /* ignore and retry */ }

                await Task.Delay(3000); // wait 3 seconds before next try
            }

            throw new Exception("Failed to authenticate test client after multiple retries.");
        }
    }
}
