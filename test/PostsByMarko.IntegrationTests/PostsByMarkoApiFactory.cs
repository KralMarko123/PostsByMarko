using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Responses;
using PostsByMarko.Host.Data;
using PostsByMarko.Host.Data.Entities;
using PostsByMarko.Host.Extensions;
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
        public string jwtToken = string.Empty;

        public async Task InitializeAsync()
        {
            client = CreateClient( new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            await RecreateAndSeedDatabaseAsync();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");
        }

        public new Task DisposeAsync()
        {
            client?.Dispose();

            return Task.CompletedTask;
        }

        public T Resolve<T>()
        {
            var scope = Services.CreateScope();

            return scope.ServiceProvider.GetRequiredService<T>();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var userManager = Resolve<UserManager<User>>();
            var user = await userManager.FindByEmailAsync(email);

            return user ?? throw new Exception("User for testing was not found.");
        }

        public async Task RecreateAndSeedDatabaseAsync()
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();
            await db.Seed();
        }

        public async Task AuthenticateClientAsync(HttpClient client)
        {
            var result = await client.PostAsJsonAsync("api/auth/login",
                new LoginDto
                {
                    Email = TestingConstants.TEST_ADMIN_EMAIL,
                    Password = TestingConstants.TEST_PASSWORD
                });

            var response = await result.Content.ReadFromJsonAsync<LoginResponse>();
            var token = response!.Token;

            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            jwtToken = token!;
        }

        public HubConnection CreateHubConnection(string hubPath)
        {
            return new HubConnectionBuilder()
                .WithUrl($"{Server.BaseAddress}{hubPath}", options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(jwtToken)!;
                    options.HttpMessageHandlerFactory = _ => Server.CreateHandler();
                })
                .WithAutomaticReconnect()
                .Build();
        }
        
        public Task WaitForSignalRPropagation() => Task.Delay(150); // Small wait for SignalR messages to propagate
    }
}
