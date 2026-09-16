using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PostsByMarko.Host.Application.Constants;
using PostsByMarko.Host.Data.Entities;
using PostsByMarko.Host.Extensions;

namespace PostsByMarko.UnitTests;

public class DatabaseInitializationTests
{
    private readonly Mock<RoleManager<IdentityRole<Guid>>> roles = new(
        Mock.Of<IRoleStore<IdentityRole<Guid>>>(), null!, null!, null!, null!);
    private readonly Mock<UserManager<User>> users = new(
        Mock.Of<IUserStore<User>>(), Options.Create(new IdentityOptions()),
        Mock.Of<IPasswordHasher<User>>(), Array.Empty<IUserValidator<User>>(),
        Array.Empty<IPasswordValidator<User>>(), Mock.Of<ILookupNormalizer>(),
        new IdentityErrorDescriber(), null!, Mock.Of<ILogger<UserManager<User>>>());

    private async Task Initialize(bool development)
    {
        using var services = new ServiceCollection()
            .AddSingleton(roles.Object).AddSingleton(users.Object).BuildServiceProvider();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["DevelopmentAdmin:Email"] = "admin@example.test",
            ["DevelopmentAdmin:Password"] = "Test-password-123!"
        }).Build();
        await services.InitializePersistentIdentityAsync(config, development);
    }

    public DatabaseInitializationTests()
    {
        roles.Setup(r => r.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
    }

    [Fact]
    public async Task Production_ignores_development_administrator_settings()
    {
        await Initialize(false);
        users.Verify(u => u.FindByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Existing_administrator_is_not_recreated_or_given_a_new_password()
    {
        var existing = new User { Email = "admin@example.test" };
        users.Setup(u => u.FindByEmailAsync(existing.Email)).ReturnsAsync(existing);
        users.Setup(u => u.IsInRoleAsync(existing, RoleConstants.ADMIN)).ReturnsAsync(true);
        await Initialize(true);
        users.Verify(u => u.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        users.Verify(u => u.ResetPasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Existing_regular_account_cannot_be_promoted_by_bootstrap()
    {
        users.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());
        await Assert.ThrowsAsync<InvalidOperationException>(() => Initialize(true));
        users.Verify(u => u.AddToRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()), Times.Never);
    }

    [Fact]
    public async Task Fresh_development_creates_roles_and_confirmed_administrator()
    {
        roles.Setup(r => r.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        roles.Setup(r => r.CreateAsync(It.IsAny<IdentityRole<Guid>>())).ReturnsAsync(IdentityResult.Success);
        users.Setup(u => u.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        users.Setup(u => u.AddToRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Success);
        await Initialize(true);
        roles.Verify(r => r.CreateAsync(It.IsAny<IdentityRole<Guid>>()), Times.Exactly(2));
        users.Verify(u => u.CreateAsync(It.Is<User>(u => u.EmailConfirmed && u.Email == "admin@example.test"),
            "Test-password-123!"), Times.Once);
        users.Verify(u => u.AddToRolesAsync(It.IsAny<User>(), It.Is<IEnumerable<string>>(r =>
            r.Contains(RoleConstants.ADMIN) && r.Contains(RoleConstants.USER))), Times.Once);
    }
}
