using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        public HttpClient? authenticatedClient;
        public HttpClient? unauthenticatedClient;
        public UserManager<User>? userManager;
        public IMapper mapper;
        public async Task InitializeAsync()
        {
            unauthenticatedClient = CreateClient( new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            authenticatedClient = CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            userManager = Services.GetRequiredService<UserManager<User>>();
            mapper = Services.GetRequiredService<IMapper>();

            await ConfigureAuthenticatedClient();
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
            unauthenticatedClient?.Dispose();
            authenticatedClient?.Dispose();

            return Task.CompletedTask;
        }

        private async Task ConfigureAuthenticatedClient()
        {
            const int maxRetries = 5;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    var result = await authenticatedClient!.PostAsJsonAsync("api/auth/login", new LoginDto { Email = TestingConstants.TEST_ADMIN.Email, Password = TestingConstants.TEST_PASSWORD });

                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadFromJsonAsync<LoginResponse>();
                        var token = response!.Token;

                        authenticatedClient!.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                        
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
