using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PostsByMarko.Host.Application.Constants;
using PostsByMarko.Host.Data;
using PostsByMarko.Host.Data.Entities;
using PostsByMarko.Host.Extensions;
using Xunit;

namespace PostsByMarko.IntegrationTests;

[Collection("IntegrationCollection")]
public class DatabaseInitializationTests(PostsByMarkoApiFactory factory) : IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync() => factory.RecreateAndSeedDatabaseAsync();

    [Fact]
    public async Task Persistent_initialization_migrates_and_preserves_existing_accounts()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal("postsbymarko_test", db.Database.GetDbConnection().Database);
        await db.Database.EnsureDeletedAsync();
        await db.Database.MigrateAsync();

        var settings = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["DevelopmentAdmin:Email"] = "bootstrap@example.test",
            ["DevelopmentAdmin:Password"] = "Local-Test-123!"
        }).Build();
        await scope.ServiceProvider.InitializePersistentIdentityAsync(settings, true);
        var users = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var admin = await users.FindByEmailAsync("bootstrap@example.test");
        Assert.NotNull(admin);
        Assert.True(await users.IsInRoleAsync(admin, RoleConstants.ADMIN));
        var originalId = admin.Id;

        settings["DevelopmentAdmin:Password"] = "Changed-Test-456!";
        await db.Database.MigrateAsync();
        await scope.ServiceProvider.InitializePersistentIdentityAsync(settings, true);
        Assert.Equal(originalId, (await users.FindByEmailAsync("bootstrap@example.test"))!.Id);
        Assert.True(await users.CheckPasswordAsync(admin, "Local-Test-123!"));
        Assert.Equal(1, await db.Users.CountAsync());
        Assert.Equal(2, await db.Roles.CountAsync());
    }
}
