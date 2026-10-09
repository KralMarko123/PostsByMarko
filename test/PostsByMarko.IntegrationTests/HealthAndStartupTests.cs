using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PostsByMarko.Host.Application.Configuration;
using PostsByMarko.Host.Application.Health;
using PostsByMarko.Host.Data;
using PostsByMarko.Host.Data.Entities;
using PostsByMarko.Test.Shared.Constants;
using Xunit;

namespace PostsByMarko.IntegrationTests;

[Collection("IntegrationCollection")]
public class HealthAndStartupTests(PostsByMarkoApiFactory factory)
{
    [Fact]
    public async Task probes_are_anonymous_and_uncached_while_the_api_remains_protected()
    {
        // Arrange
        using var client = factory.CreateClient();
        // Act
        foreach (var path in new[] { "/health/live", "/health/ready" })
        {
            using var response = await client.GetAsync(path);
            // Assert
            await AssertProbeAsync(response, HttpStatusCode.OK, "Healthy");
        }
        using var apiResponse = await client.GetAsync("/api/user/all");
        Assert.Equal(HttpStatusCode.Unauthorized, apiResponse.StatusCode);
    }

    [Fact]
    public async Task unavailable_database_fails_readiness_but_liveness_survives_and_readiness_recovers()
    {
        // Arrange
        var state = new DatabaseAvailability();
        using var probeFactory = factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
        {
            services.AddSingleton(state);
            services.AddScoped<ProbeDbContext>(provider =>
            {
                var connection = provider.GetRequiredService<DatabaseAvailability>().Unavailable
                    ? "server=127.0.0.1;port=1;database=health_probe_test;user=health_probe;password=health-probe-secret;connectiontimeout=1"
                    : provider.GetRequiredService<IOptions<DatabaseConfig>>().Value.DefaultConnection;
                return new ProbeDbContext(new DbContextOptionsBuilder<AppDbContext>()
                    .UseMySql(connection, new MariaDbServerVersion(new Version(10, 11, 13))).Options,
                    provider.GetRequiredService<IHostEnvironment>());
            });
            // Exercise the real health check against working and refused MariaDB connections.
            services.AddScoped(provider => new DatabaseReadinessHealthCheck(provider.GetRequiredService<ProbeDbContext>()));
        }));
        using var client = probeFactory.CreateClient();
        // Act
        using (var ready = await client.GetAsync("/health/ready"))
            // Assert
            await AssertProbeAsync(ready, HttpStatusCode.OK, "Healthy");
        state.Unavailable = true;
        using (var unavailable = await client.GetAsync("/health/ready"))
            await AssertProbeAsync(unavailable, HttpStatusCode.ServiceUnavailable, "Unhealthy");
        using (var live = await client.GetAsync("/health/live"))
            await AssertProbeAsync(live, HttpStatusCode.OK, "Healthy");
        state.Unavailable = false;
        using var recovered = await client.GetAsync("/health/ready");
        await AssertProbeAsync(recovered, HttpStatusCode.OK, "Healthy");
    }

    [Theory]
    [InlineData("ApplicationUrls:ApiBaseUrl", "ftp://example.test")]
    [InlineData("ApplicationUrls:ClientBaseUrl", "https://user:startup-validation-secret@example.test")]
    [InlineData("EmailConfig:Host", "")]
    [InlineData("EmailConfig:Port", "0")]
    [InlineData("EmailConfig:SenderAddress", "startup-validation-secret")]
    [InlineData("ConnectionStrings:DefaultConnection", "unsupported=startup-validation-secret")]
    [InlineData("JwtConfig:ExpiresInMinutes", "0")]
    public async Task invalid_startup_settings_are_rejected_before_database_initialization(string key, string value)
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var author = await db.Users.FirstAsync(user => user.Email == TestingConstants.TEST_ADMIN_EMAIL);
        var sentinel = new Post { Id = Guid.NewGuid(), AuthorId = author.Id, Title = "Startup sentinel", Content = "Must survive failed startup" };
        db.Posts.Add(sentinel);
        await db.SaveChangesAsync();
        var settings = new Dictionary<string, string?> { [key] = value };
        if (key.StartsWith("EmailConfig:", StringComparison.Ordinal))
        {
            settings["EmailConfig:Enabled"] = "true";
            settings.TryAdd("EmailConfig:Host", "mailpit");
            settings.TryAdd("EmailConfig:Port", "1025");
            settings.TryAdd("EmailConfig:SenderAddress", "local@postsbymarko.test");
            settings["EmailConfig:Username"] = "";
            settings["EmailConfig:Password"] = "";
        }
        using var invalidFactory = factory.WithWebHostBuilder(builder =>
            builder.ConfigureAppConfiguration((_, configuration) => configuration.AddInMemoryCollection(settings)));
        // Act
        var exception = Assert.Throws<OptionsValidationException>(() => invalidFactory.CreateClient());
        // Assert
        Assert.Contains(key, exception.Message);
        Assert.DoesNotContain("startup-validation-secret", exception.ToString());
        using var verification = factory.Services.CreateScope();
        Assert.True(await verification.ServiceProvider.GetRequiredService<AppDbContext>().Posts.AnyAsync(post => post.Id == sentinel.Id));
    }

    private static async Task AssertProbeAsync(HttpResponseMessage response, HttpStatusCode status, string body)
    {
        Assert.Equal(status, response.StatusCode);
        Assert.Equal("text/plain", response.Content.Headers.ContentType!.MediaType);
        Assert.Equal(body, await response.Content.ReadAsStringAsync());
        Assert.True(response.Headers.CacheControl!.NoStore);
        Assert.Null(response.Headers.Location);
        Assert.Empty(response.Headers.WwwAuthenticate);
    }

    private sealed class DatabaseAvailability
    {
        public bool Unavailable { get; set; }
    }

    private sealed class ProbeDbContext(DbContextOptions options, IHostEnvironment environment) : AppDbContext(options, environment);
}
