using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PostsByMarko.Host.Application.Configuration;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Responses;
using PostsByMarko.Host.Data;
using PostsByMarko.Host.Data.Entities;
using PostsByMarko.Test.Shared.Constants;
using Xunit;

namespace PostsByMarko.IntegrationTests;

[Collection("IntegrationCollection")]
public class ApiErrorContractTests(PostsByMarkoApiFactory factory) : IAsyncLifetime
{
    public Task InitializeAsync() => factory.RecreateAndSeedDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task rejected_tokens_return_a_bearer_challenge_and_generic_problem_details()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var user = (await users.FindByEmailAsync(TestingConstants.TEST_USER_EMAIL))!;
        var config = scope.ServiceProvider.GetRequiredService<IOptions<JwtConfig>>().Value;
        using var client = await LoginAsync(TestingConstants.TEST_USER_EMAIL);
        // Act
        using (var valid = await client.GetAsync("/api/auth/validate"))
            // Assert
            Assert.Equal(HttpStatusCode.OK, valid.StatusCode);
        client.DefaultRequestHeaders.Authorization = new("Bearer", CreateToken(user, config));
        using (var generated = await client.GetAsync("/api/auth/validate"))
            Assert.Equal(HttpStatusCode.OK, generated.StatusCode);

        var invalidTokens = new string?[]
        {
            null, "not-a-jwt",
            CreateToken(user, config, expires: DateTime.UtcNow.AddMinutes(-20)),
            CreateToken(user, config, key: "a-different-test-only-signing-key-with-at-least-32-characters"),
            CreateToken(user, config, issuer: "invalid-issuer"),
            CreateToken(user, config, audience: "invalid-audience"),
            CreateToken(user, config, includeStamp: false),
            CreateToken(user, config, stamp: "revoked-stamp")
        };
        foreach (var token in invalidTokens)
        {
            client.DefaultRequestHeaders.Authorization = token is null ? null : new("Bearer", token);
            using var response = await client.GetAsync("/api/auth/validate");
            var problem = await AssertProblemAsync(response, HttpStatusCode.Unauthorized, "authentication_required", "/api/auth/validate");
            Assert.Equal("A valid access token is required.", problem.GetProperty("detail").GetString());
            Assert.False(problem.TryGetProperty("error_description", out _));
        }

        var previousToken = CreateToken(user, config);
        Assert.True((await users.UpdateSecurityStampAsync(user)).Succeeded);
        client.DefaultRequestHeaders.Authorization = new("Bearer", previousToken);
        using (var revoked = await client.GetAsync("/api/auth/validate"))
            await AssertProblemAsync(revoked, HttpStatusCode.Unauthorized, "authentication_required", "/api/auth/validate");

        // A token issued after revocation is accepted until its account is deleted.
        client.DefaultRequestHeaders.Authorization = new("Bearer", CreateToken(user, config));
        using (var current = await client.GetAsync("/api/auth/validate"))
            Assert.Equal(HttpStatusCode.OK, current.StatusCode);
        Assert.True((await users.DeleteAsync(user)).Succeeded);
        using var deleted = await client.GetAsync("/api/auth/validate");
        await AssertProblemAsync(deleted, HttpStatusCode.Unauthorized, "authentication_required", "/api/auth/validate");
    }

    [Fact]
    public async Task invalid_login_credentials_also_return_a_bearer_challenge()
    {
        // Arrange
        using var client = factory.CreateClient();
        // Act
        foreach (var email in new[] { TestingConstants.TEST_USER_EMAIL, "missing@example.test" })
        {
            using var response = await client.PostAsJsonAsync("/api/auth/login", new LoginDto
            {
                Email = email,
                Password = "Invalid-password-123"
            });
            // Assert
            var problem = await AssertProblemAsync(response, HttpStatusCode.Unauthorized, "authentication_failed", "/api/auth/login");
            Assert.Equal("Invalid email or password.", problem.GetProperty("detail").GetString());
        }
    }

    [Fact]
    public async Task validation_and_service_errors_share_the_400_contract()
    {
        // Arrange
        using var client = await LoginAsync(TestingConstants.TEST_ADMIN_EMAIL);
        // Act
        using (var invalid = await client.PostAsJsonAsync("/api/post", new { title = "", content = "" }))
        {
            // Assert
            var problem = await AssertProblemAsync(invalid, HttpStatusCode.BadRequest, "validation_failed", "/api/post");
            Assert.True(problem.GetProperty("errors").EnumerateObject().Any());
        }
        using (var malformed = await client.PostAsync("/api/auth/login", new StringContent("{", Encoding.UTF8, "application/json")))
        {
            var problem = await AssertProblemAsync(malformed, HttpStatusCode.BadRequest, "validation_failed", "/api/auth/login");
            Assert.True(problem.GetProperty("errors").EnumerateObject().Any());
        }
        using (var invalidPage = await client.GetAsync("/api/post/all?page=0"))
            await AssertProblemAsync(invalidPage, HttpStatusCode.BadRequest, "validation_failed", "/api/post/all");
        var admin = await factory.GetUserByEmailAsync(TestingConstants.TEST_ADMIN_EMAIL);
        using var selfDeletion = await client.DeleteAsync($"/api/admin/users/{admin.Id}");
        await AssertProblemAsync(selfDeletion, HttpStatusCode.BadRequest, "invalid_request", $"/api/admin/users/{admin.Id}");
    }

    [Fact]
    public async Task authorization_resource_and_routing_failures_preserve_their_status_and_headers()
    {
        // Arrange
        using var client = await LoginAsync(TestingConstants.TEST_USER_EMAIL);
        // Act
        using (var forbidden = await client.GetAsync("/api/admin/dashboard"))
            // Assert
            await AssertProblemAsync(forbidden, HttpStatusCode.Forbidden, "forbidden", "/api/admin/dashboard");

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var admin = await factory.GetUserByEmailAsync(TestingConstants.TEST_ADMIN_EMAIL);
        var hidden = new Post { Id = Guid.NewGuid(), AuthorId = admin.Id, Title = "Private", Content = "Private", Hidden = true };
        db.Posts.Add(hidden);
        await db.SaveChangesAsync();
        using (var serviceForbidden = await client.GetAsync($"/api/post/{hidden.Id}"))
            await AssertProblemAsync(serviceForbidden, HttpStatusCode.Forbidden, "forbidden", $"/api/post/{hidden.Id}");

        var missingPath = $"/api/post/{Guid.NewGuid()}";
        using (var missing = await client.GetAsync(missingPath))
            await AssertProblemAsync(missing, HttpStatusCode.NotFound, "resource_not_found", missingPath);
        using (var unknownRoute = await client.GetAsync("/api/unknown-endpoint"))
            await AssertProblemAsync(unknownRoute, HttpStatusCode.NotFound, "endpoint_not_found", "/api/unknown-endpoint");
        using var wrongMethod = await client.PostAsync("/api/post/all", null);
        await AssertProblemAsync(wrongMethod, HttpStatusCode.MethodNotAllowed, "method_not_allowed", "/api/post/all");
        Assert.Contains("GET", wrongMethod.Content.Headers.Allow);
    }

    [Fact]
    public async Task duplicate_registration_returns_a_409_problem_without_identity_internals()
    {
        // Arrange
        using var client = factory.CreateClient();
        // Act
        using var response = await client.PostAsJsonAsync("/api/auth/register", new RegistrationDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = TestingConstants.TEST_USER_EMAIL,
            Password = TestingConstants.TEST_PASSWORD
        });
        // Assert
        var problem = await AssertProblemAsync(response, HttpStatusCode.Conflict, "conflict", "/api/auth/register");
        Assert.Equal("An account with this email already exists.", problem.GetProperty("detail").GetString());
    }

    [Fact]
    public async Task authentication_rate_limit_returns_a_429_problem_across_login_and_registration()
    {
        // This host has a small limit and uses the same disposable database. Collection execution is serial.
        // Arrange
        using var limitedFactory = new RateLimitedApiFactory();
        using var client = limitedFactory.CreateClient();
        // Act
        using (var login = await client.PostAsJsonAsync("/api/auth/login", new { }))
            // Assert
            await AssertProblemAsync(login, HttpStatusCode.BadRequest, "validation_failed", "/api/auth/login");
        using (var registration = await client.PostAsJsonAsync("/api/auth/register", new { }))
            await AssertProblemAsync(registration, HttpStatusCode.BadRequest, "validation_failed", "/api/auth/register");
        using var throttled = await client.PostAsJsonAsync("/api/auth/login", new { });
        await AssertProblemAsync(throttled, HttpStatusCode.TooManyRequests, "rate_limit_exceeded", "/api/auth/login");
    }

    private async Task<HttpClient> LoginAsync(string email)
    {
        var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync("/api/auth/login", new LoginDto { Email = email, Password = TestingConstants.TEST_PASSWORD });
        response.EnsureSuccessStatusCode();
        client.DefaultRequestHeaders.Authorization = new("Bearer", (await response.Content.ReadFromJsonAsync<LoginResponse>())!.Token);
        return client;
    }

    private static string CreateToken(User user, JwtConfig config, DateTime? expires = null, string? key = null,
        string? issuer = null, string? audience = null, bool includeStamp = true, string? stamp = null)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.PrimarySid, user.Id.ToString())
        };
        if (includeStamp)
            claims.Add(new("AspNet.Identity.SecurityStamp", stamp ?? user.SecurityStamp!));
        var expiration = expires ?? DateTime.UtcNow.AddMinutes(30);
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = issuer ?? config.ValidIssuers.First(),
            Audience = audience ?? config.ValidAudiences.First(),
            Expires = expiration,
            NotBefore = expiration.AddMinutes(-30),
            IssuedAt = expiration.AddMinutes(-30),
            SigningCredentials = new(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key ?? config.Secret)), SecurityAlgorithms.HmacSha256)
        };
        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(handler.CreateToken(descriptor));
    }

    private static async Task<JsonElement> AssertProblemAsync(HttpResponseMessage response, HttpStatusCode status, string code, string path)
    {
        Assert.Equal(status, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType!.MediaType);
        using var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var problem = body.RootElement;
        Assert.Equal((int)status, problem.GetProperty("status").GetInt32());
        Assert.Equal("about:blank", problem.GetProperty("type").GetString());
        Assert.Equal(path, problem.GetProperty("instance").GetString());
        Assert.Equal(code, problem.GetProperty("code").GetString());
        Assert.False(string.IsNullOrWhiteSpace(problem.GetProperty("title").GetString()));
        Assert.False(string.IsNullOrWhiteSpace(problem.GetProperty("traceId").GetString()));
        var detail = problem.GetProperty("detail").GetString();
        Assert.False(string.IsNullOrWhiteSpace(detail));
        Assert.Equal(detail, problem.GetProperty("message").GetString());
        Assert.False(problem.TryGetProperty("exception", out _));
        Assert.False(problem.TryGetProperty("stackTrace", out _));
        Assert.Null(response.Headers.Location);
        if (status == HttpStatusCode.Unauthorized)
        {
            var challenge = Assert.Single(response.Headers.WwwAuthenticate);
            Assert.Equal("Bearer", challenge.Scheme);
            Assert.Null(challenge.Parameter);
        }
        else
            Assert.Empty(response.Headers.WwwAuthenticate);
        return problem.Clone();
    }

    private sealed class RateLimitedApiFactory : PostsByMarkoApiFactory
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.UseSetting("Authentication:RequestsPerMinute", "2");
        }
    }
}
