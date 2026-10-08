using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PostsByMarko.Host.Application.Configuration;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Requests;
using PostsByMarko.Host.Application.Responses;
using PostsByMarko.Host.Data;
using PostsByMarko.Host.Data.Entities;
using PostsByMarko.Host.Data.Repositories.Messaging;
using PostsByMarko.Host.Data.Repositories.Users;
using PostsByMarko.Host.Extensions;
using PostsByMarko.Test.Shared.Constants;
using Xunit;

namespace PostsByMarko.IntegrationTests;

[Collection("IntegrationCollection")]
public class PaginationTests(PostsByMarkoApiFactory factory) : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await factory.RecreateAndSeedDatabaseAsync();
        await factory.AuthenticateClientAsync(factory.client!);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task post_visibility_is_filtered_before_paging_and_counts_exclude_hidden_posts()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var viewer = (await users.FindByEmailAsync(TestingConstants.TEST_USER_EMAIL))!;
        var admin = (await users.FindByEmailAsync(TestingConstants.TEST_ADMIN_EMAIL))!;
        await db.Posts.ExecuteDeleteAsync();
        var date = new DateTime(2026, 10, 1, 12, 0, 0, DateTimeKind.Utc);
        var posts = Enumerable.Range(0, 120).Select(index => new Post
        {
            Id = Guid.NewGuid(), AuthorId = admin.Id, Title = $"Post {index}", Content = "Content",
            CreatedAt = date.AddSeconds(index / 3), LastUpdatedAt = date
        }).ToList();
        posts.Add(new Post { Id = Guid.NewGuid(), AuthorId = admin.Id, Hidden = true, Title = "Private", Content = "Private", CreatedAt = date.AddHours(1) });
        posts.Add(new Post { Id = Guid.NewGuid(), AuthorId = viewer.Id, Hidden = true, Title = "Own private", Content = "Own", CreatedAt = date.AddHours(2) });
        db.Posts.AddRange(posts);
        await db.SaveChangesAsync();
        using var client = await LoginAsync(TestingConstants.TEST_USER_EMAIL);
        var expected = posts.Where(post => !post.Hidden || post.AuthorId == viewer.Id)
            .OrderByDescending(post => post.CreatedAt).ThenByDescending(post => post.Id.ToString(), StringComparer.Ordinal)
            .Select(post => post.Id).ToArray();
        var received = new List<Guid>();
        for (var page = 1; page <= 3; page++)
        {
            using var response = await client.GetAsync($"/api/post/all?page={page}");
            response.EnsureSuccessStatusCode();
            Assert.Equal("121", response.Headers.GetValues("X-Total-Count").Single());
            Assert.Equal("50", response.Headers.GetValues("X-Page-Size").Single());
            Assert.Equal(page < 3 ? "true" : "false", response.Headers.GetValues("X-Has-Next-Page").Single());
            var items = (await response.Content.ReadFromJsonAsync<List<PostDto>>())!;
            Assert.Equal(expected.Skip((page - 1) * 50).Take(50), items.Select(post => post.Id));
            received.AddRange(items.Select(post => post.Id));
        }
        Assert.Equal(121, received.Distinct().Count());
        using var adminResponse = await factory.client!.GetAsync("/api/post/all?pageSize=100");
        adminResponse.EnsureSuccessStatusCode();
        Assert.Equal("122", adminResponse.Headers.GetValues("X-Total-Count").Single());
        Assert.Equal(100, (await adminResponse.Content.ReadFromJsonAsync<List<PostDto>>())!.Count);
        using var empty = await client.GetAsync("/api/post/all?page=999");
        Assert.Empty((await empty.Content.ReadFromJsonAsync<List<PostDto>>())!);
        Assert.Equal("121", empty.Headers.GetValues("X-Total-Count").Single());
    }

    [Fact]
    public async Task user_and_dashboard_pages_are_projected_without_loading_post_graphs_or_querying_roles_per_user()
    {
        using var scope = factory.Services.CreateScope();
        var services = scope.ServiceProvider;
        var db = services.GetRequiredService<AppDbContext>();
        var manager = services.GetRequiredService<UserManager<User>>();
        var admin = (await manager.FindByEmailAsync(TestingConstants.TEST_ADMIN_EMAIL))!;
        var role = await db.Roles.SingleAsync(role => role.Name == "User");
        var noPosts = new User { Id = Guid.NewGuid(), UserName = "a-empty@example.test", Email = "a-empty@example.test" };
        var withPosts = new User { Id = Guid.NewGuid(), UserName = "a-posts@example.test", Email = "a-posts@example.test" };
        db.Users.AddRange(noPosts, withPosts);
        db.UserRoles.Add(new IdentityUserRole<Guid> { UserId = withPosts.Id, RoleId = role.Id });
        var lastUpdated = new DateTime(2026, 10, 2, 12, 0, 0, DateTimeKind.Utc);
        db.Posts.AddRange(new Post { Id = Guid.NewGuid(), AuthorId = withPosts.Id, Title = "First", Content = "Content", LastUpdatedAt = lastUpdated.AddHours(-1) },
            new Post { Id = Guid.NewGuid(), AuthorId = withPosts.Id, Title = "Second", Content = "Content", LastUpdatedAt = lastUpdated, Hidden = true });
        await db.SaveChangesAsync();
        var total = await db.Users.CountAsync(user => user.Id != admin.Id);
        var commands = new CommandCapture();
        await using var queryDb = CreateQueryContext(services, commands);
        var repository = new UserRepository(queryDb, manager);

        var dashboard = await repository.GetAdminDashboardAsync(admin.Id, new PageRequest { PageSize = 2 });
        Assert.Equal(total, dashboard.TotalCount);
        Assert.Equal(2, dashboard.Items.Count);
        var empty = Assert.Single(dashboard.Items, row => row.UserId == noPosts.Id);
        Assert.Equal(0, empty.NumberOfPosts);
        Assert.Null(empty.LastPostedAt);
        Assert.Empty(empty.Roles);
        var populated = Assert.Single(dashboard.Items, row => row.UserId == withPosts.Id);
        Assert.Equal(2, populated.NumberOfPosts);
        Assert.Equal(lastUpdated, populated.LastPostedAt);
        Assert.Equal(new[] { "User" }, populated.Roles);
        Assert.Equal(3, commands.Commands.Count);
        Assert.Contains(commands.Commands, sql => sql.Contains("MAX(") && sql.Contains("COUNT("));
        Assert.Empty(queryDb.ChangeTracker.Entries());
        commands.Commands.Clear();

        var userPage = await repository.GetUsersAsync(new PageRequest { PageSize = 2 }, admin.Id);
        Assert.Equal(total, userPage.TotalCount);
        Assert.Equal(new[] { noPosts.Id, withPosts.Id }, userPage.Items.Select(user => user.Id!.Value));
        Assert.Equal(2, commands.Commands.Count);
        Assert.DoesNotContain(commands.Commands, sql => sql.Contains("PasswordHash") || sql.Contains("SecurityStamp") || sql.Contains("Posts"));
        Assert.Empty(queryDb.ChangeTracker.Entries());

        using var httpResponse = await factory.client!.GetAsync($"/api/user/all?pageSize=2&exceptId={admin.Id}");
        httpResponse.EnsureSuccessStatusCode();
        Assert.Equal(total.ToString(), httpResponse.Headers.GetValues("X-Total-Count").Single());
        Assert.Equal(2, (await httpResponse.Content.ReadFromJsonAsync<List<UserDto>>())!.Count);
        using var dashboardResponse = await factory.client.GetAsync("/api/admin/dashboard?pageSize=2");
        dashboardResponse.EnsureSuccessStatusCode();
        Assert.Equal(total.ToString(), dashboardResponse.Headers.GetValues("X-Total-Count").Single());
        Assert.Equal(2, (await dashboardResponse.Content.ReadFromJsonAsync<List<AdminDashboardResponse>>())!.Count);
    }

    [Fact]
    public async Task chat_summaries_and_recent_windows_are_bounded_and_history_is_paged_for_members_only()
    {
        using var scope = factory.Services.CreateScope();
        var services = scope.ServiceProvider;
        var db = services.GetRequiredService<AppDbContext>();
        var users = services.GetRequiredService<UserManager<User>>();
        var admin = (await users.FindByEmailAsync(TestingConstants.TEST_ADMIN_EMAIL))!;
        var recipient = (await users.FindByEmailAsync(TestingConstants.TEST_USER_EMAIL))!;
        var outsider = (await users.FindByEmailAsync(TestingConstants.OWNER_EMAIL))!;
        var unconfirmed = (await users.FindByEmailAsync(TestingConstants.UNCONFIRMED_USER_EMAIL))!;
        await db.Chats.ExecuteDeleteAsync();
        var date = new DateTime(2026, 10, 1, 12, 0, 0, DateTimeKind.Utc);
        Chat CreateChat(Guid first, Guid second) => new()
        {
            Id = Guid.NewGuid(), CreatedAt = date, UpdatedAt = date,
            ChatUsers = [new() { UserId = first }, new() { UserId = second }]
        };
        var chat = CreateChat(admin.Id, recipient.Id);
        var otherChat = CreateChat(admin.Id, outsider.Id);
        var privateChat = CreateChat(recipient.Id, unconfirmed.Id);
        chat.Messages = Enumerable.Range(0, 103).Select(index => new Message
        {
            Id = Guid.NewGuid(), ChatId = chat.Id, SenderId = admin.Id, Content = $"Message {index}", CreatedAt = date.AddSeconds(index / 3)
        }).ToList();
        db.Chats.AddRange(chat, otherChat, privateChat);
        await db.SaveChangesAsync();
        var expected = chat.Messages.OrderByDescending(message => message.CreatedAt)
            .ThenByDescending(message => message.Id.ToString(), StringComparer.Ordinal).Select(message => message.Id).ToArray();
        var commands = new CommandCapture();
        await using var queryDb = CreateQueryContext(services, commands);
        var repository = new ChatRepository(queryDb);
        var firstPage = await repository.GetChatsForUserAsync(admin.Id, new PageRequest { PageSize = 1 }, default);
        var secondPage = await repository.GetChatsForUserAsync(admin.Id, new PageRequest { Page = 2, PageSize = 1 }, default);
        Assert.Equal(2, firstPage.TotalCount);
        Assert.Equal(new[] { chat.Id, otherChat.Id }.OrderByDescending(id => id.ToString(), StringComparer.Ordinal),
            firstPage.Items.Concat(secondPage.Items).Select(item => item.Id));
        var summary = Assert.Single(firstPage.Items.Concat(secondPage.Items), item => item.Id == chat.Id);
        Assert.Equal(103, summary.MessageCount);
        Assert.True(summary.HasMoreMessages);
        Assert.Equal(expected[0], Assert.Single(summary.Messages).Id);
        Assert.Empty(queryDb.ChangeTracker.Entries());
        Assert.Contains(commands.Commands, sql => sql.Contains("ROW_NUMBER()") || sql.Contains("LIMIT"));
        using var listResponse = await factory.client!.GetAsync("/api/messaging/chats?pageSize=1");
        listResponse.EnsureSuccessStatusCode();
        Assert.Equal("2", listResponse.Headers.GetValues("X-Total-Count").Single());
        Assert.Single((await listResponse.Content.ReadFromJsonAsync<List<ChatDto>>())!);

        using var opened = await factory.client.PostAsync($"/api/messaging/chats/user/{recipient.Id}", null);
        opened.EnsureSuccessStatusCode();
        var detail = (await opened.Content.ReadFromJsonAsync<ChatDto>())!;
        Assert.Equal(103, detail.MessageCount);
        Assert.True(detail.HasMoreMessages);
        Assert.Equal(expected.Take(50).Reverse(), detail.Messages.Select(message => message.Id!.Value));
        var loaded = new List<Guid>();
        for (var page = 1; page <= 3; page++)
        {
            using var response = await factory.client.GetAsync($"/api/messaging/chats/{chat.Id}/messages?page={page}");
            response.EnsureSuccessStatusCode();
            Assert.Equal("103", response.Headers.GetValues("X-Total-Count").Single());
            Assert.Equal(page < 3 ? "true" : "false", response.Headers.GetValues("X-Has-Next-Page").Single());
            var items = (await response.Content.ReadFromJsonAsync<List<MessageDto>>())!;
            Assert.Equal(expected.Skip((page - 1) * 50).Take(50).Reverse(), items.Select(message => message.Id!.Value));
            loaded.AddRange(items.Select(message => message.Id!.Value));
        }
        Assert.Equal(103, loaded.Distinct().Count());
        using var outsiderClient = await LoginAsync(TestingConstants.OWNER_EMAIL);
        using var forbidden = await outsiderClient.GetAsync($"/api/messaging/chats/{chat.Id}/messages");
        Assert.Equal(HttpStatusCode.Forbidden, forbidden.StatusCode);
        using var missing = await factory.client.GetAsync($"/api/messaging/chats/{Guid.NewGuid()}/messages");
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
    }

    [Fact]
    public async Task every_list_rejects_invalid_paging_and_exposes_metadata_to_the_browser()
    {
        foreach (var endpoint in new[] { "/api/post/all", "/api/user/all", "/api/admin/dashboard", "/api/messaging/chats" })
        foreach (var query in new[] { "page=0", "pageSize=0", "pageSize=101", "page=2147483647&pageSize=100", "page=bad" })
        {
            using var response = await factory.client!.GetAsync($"{endpoint}?{query}");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("application/problem+json", response.Content.Headers.ContentType!.MediaType);
        }
        using var scope = factory.Services.CreateScope();
        var origin = scope.ServiceProvider.GetRequiredService<IOptions<JwtConfig>>().Value.ValidAudiences.First();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/user/all?pageSize=1");
        request.Headers.Add("Origin", origin);
        using var result = await factory.client!.SendAsync(request);
        result.EnsureSuccessStatusCode();
        var exposed = string.Join(",", result.Headers.GetValues("Access-Control-Expose-Headers"));
        foreach (var header in PaginationResponseExtensions.HeaderNames) Assert.Contains(header, exposed);
    }

    [Fact]
    public async Task pagination_indexes_migrate_up_and_down_without_losing_existing_data()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal("postsbymarko_test", db.Database.GetDbConnection().Database);
        await db.Database.EnsureDeletedAsync();
        var migrator = db.GetService<IMigrator>();
        const string previousMigration = "20260922075150_AddEmailOutbox";
        await migrator.MigrateAsync(previousMigration);
        await db.Seed();
        var userCount = await db.Users.CountAsync();
        var postCount = await db.Posts.CountAsync();
        await db.Database.MigrateAsync();
        Assert.Equal(userCount, await db.Users.CountAsync());
        Assert.Equal(postCount, await db.Posts.CountAsync());
        Assert.False(db.Database.HasPendingModelChanges());
        await migrator.MigrateAsync(previousMigration);
        await db.Database.MigrateAsync();
        Assert.Equal(userCount, await db.Users.CountAsync());
        Assert.Equal(postCount, await db.Posts.CountAsync());
    }

    private async Task<HttpClient> LoginAsync(string email)
    {
        var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync("/api/auth/login", new LoginDto { Email = email, Password = TestingConstants.TEST_PASSWORD });
        response.EnsureSuccessStatusCode();
        client.DefaultRequestHeaders.Authorization = new("Bearer", (await response.Content.ReadFromJsonAsync<LoginResponse>())!.Token);
        return client;
    }

    private static AppDbContext CreateQueryContext(IServiceProvider services, CommandCapture capture) => new(
        new DbContextOptionsBuilder<AppDbContext>().UseMySql(services.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection"),
            new MariaDbServerVersion(new Version(10, 11, 13))).AddInterceptors(capture).Options,
        services.GetRequiredService<IHostEnvironment>());

    private sealed class CommandCapture : DbCommandInterceptor
    {
        public List<string> Commands { get; } = [];
        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command,
            CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            Commands.Add(command.CommandText);
            return ValueTask.FromResult(result);
        }
    }
}
