using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PostsByMarko.Host.Application.Constants;
using PostsByMarko.Host.Data;
using PostsByMarko.Host.Data.Entities;
using PostsByMarko.Host.Data.Repositories.Users;
using PostsByMarko.Test.Shared.Constants;
using Xunit;

namespace PostsByMarko.IntegrationTests;

[Collection("IntegrationCollection")]
public class RoleTransactionTests(PostsByMarkoApiFactory factory) : IAsyncLifetime
{
    public Task InitializeAsync() => factory.RecreateAndSeedDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Theory]
    [InlineData("add", false)]
    [InlineData("remove", false)]
    [InlineData("guarded-remove", false)]
    [InlineData("add", true)]
    [InlineData("remove", true)]
    [InlineData("guarded-remove", true)]
    public async Task stamp_failure_rolls_back_role_and_user_changes(string operation, bool throwException)
    {
        // Arrange
        Guid userId;
        string? originalStamp;
        string? originalConcurrencyStamp;
        string[] originalRoles;
        using (var scope = factory.Services.CreateScope())
        {
            using var users = new FailingStampUserManager(scope.ServiceProvider, throwException);
            var user = (await users.FindByEmailAsync(operation == "guarded-remove"
                ? TestingConstants.TEST_ADMIN_EMAIL : TestingConstants.TEST_USER_EMAIL))!;
            userId = user.Id;
            originalStamp = user.SecurityStamp;
            originalConcurrencyStamp = user.ConcurrencyStamp;
            originalRoles = (await users.GetRolesAsync(user)).OrderBy(role => role).ToArray();
            var repository = new UserRepository(scope.ServiceProvider.GetRequiredService<AppDbContext>(), users);

            // Act
            if (throwException)
            {
                await Assert.ThrowsAsync<InvalidOperationException>(() => ChangeRole(repository, user, operation));
            }
            else
            {
                Assert.False((await ChangeRole(repository, user, operation)).Succeeded);
            }
        }

        // Assert
        // Use another context so EF's tracked entities cannot hide a partial database write.
        using var verification = factory.Services.CreateScope();
        var persistedUsers = verification.ServiceProvider.GetRequiredService<UserManager<User>>();
        var persistedUser = (await persistedUsers.FindByIdAsync(userId.ToString()))!;
        Assert.Equal(originalStamp, persistedUser.SecurityStamp);
        Assert.Equal(originalConcurrencyStamp, persistedUser.ConcurrencyStamp);
        Assert.Equal(originalRoles, (await persistedUsers.GetRolesAsync(persistedUser)).OrderBy(role => role).ToArray());
    }

    [Theory]
    [InlineData("add")]
    [InlineData("remove")]
    [InlineData("guarded-remove")]
    public async Task successful_role_change_commits_membership_and_new_stamp(string operation)
    {
        // Arrange
        Guid userId;
        string? originalStamp;
        using (var scope = factory.Services.CreateScope())
        {
            var users = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var user = (await users.FindByEmailAsync(operation == "guarded-remove"
                ? TestingConstants.TEST_ADMIN_EMAIL : TestingConstants.TEST_USER_EMAIL))!;
            userId = user.Id;
            originalStamp = user.SecurityStamp;
            // Act
            var result = await ChangeRole(scope.ServiceProvider.GetRequiredService<IUserRepository>(), user, operation);

            // Assert
            Assert.True(result.Succeeded);
        }

        // Assert
        using var verification = factory.Services.CreateScope();
        var persistedUsers = verification.ServiceProvider.GetRequiredService<UserManager<User>>();
        var persistedUser = (await persistedUsers.FindByIdAsync(userId.ToString()))!;
        Assert.NotEqual(originalStamp, persistedUser.SecurityStamp);
        Assert.Equal(operation == "add", await persistedUsers.IsInRoleAsync(persistedUser,
            operation == "remove" ? RoleConstants.USER : RoleConstants.ADMIN));
    }

    private static Task<IdentityResult> ChangeRole(IUserRepository repository, User user, string operation) =>
        operation switch
        {
            "add" => repository.AddRoleToUserAsync(user, RoleConstants.ADMIN),
            "remove" => repository.RemoveRoleFromUserAsync(user, RoleConstants.USER),
            _ => repository.RemoveRoleFromUserUnlessLastMemberAsync(user, RoleConstants.ADMIN)
        };

    private sealed class FailingStampUserManager(IServiceProvider services, bool throwException)
        : UserManager<User>(
            services.GetRequiredService<IUserStore<User>>(),
            services.GetRequiredService<IOptions<IdentityOptions>>(),
            services.GetRequiredService<IPasswordHasher<User>>(),
            services.GetServices<IUserValidator<User>>(),
            services.GetServices<IPasswordValidator<User>>(),
            services.GetRequiredService<ILookupNormalizer>(),
            services.GetRequiredService<IdentityErrorDescriber>(),
            services,
            services.GetRequiredService<ILogger<UserManager<User>>>())
    {
        public override Task<IdentityResult> UpdateSecurityStampAsync(User user)
        {
            if (throwException)
                throw new InvalidOperationException("Injected security-stamp failure.");
            return Task.FromResult(IdentityResult.Failed(new IdentityError { Code = "InjectedStampFailure" }));
        }
    }
}
