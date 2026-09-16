using Microsoft.AspNetCore.Identity;
using PostsByMarko.Host.Application.Constants;
using PostsByMarko.Host.Data.Entities;

namespace PostsByMarko.Host.Extensions;

public static class DatabaseInitializationExtensions
{
    public static async Task InitializePersistentIdentityAsync(this IServiceProvider services,
        IConfiguration configuration, bool isDevelopment)
    {
        var roles = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        foreach (var role in new[] { RoleConstants.ADMIN, RoleConstants.USER })
        {
            if (!await roles.RoleExistsAsync(role))
                EnsureSuccess(await roles.CreateAsync(new IdentityRole<Guid>(role)), "Create application role");
        }

        if (!isDevelopment) return;

        var email = configuration["DevelopmentAdmin:Email"];
        var password = configuration["DevelopmentAdmin:Password"];
        if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(password)) return;
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException("DevelopmentAdmin requires both Email and Password.");

        var users = services.GetRequiredService<UserManager<User>>();
        var existing = await users.FindByEmailAsync(email);
        if (existing is not null)
        {
            if (!await users.IsInRoleAsync(existing, RoleConstants.ADMIN))
                throw new InvalidOperationException("DevelopmentAdmin email belongs to an existing non-admin account. Choose a new email.");
            return; // Never change an existing account's password on startup.
        }

        var user = new User
        {
            UserName = email, Email = email, EmailConfirmed = true,
            FirstName = "Local", LastName = "Admin"
        };
        EnsureSuccess(await users.CreateAsync(user, password), "Create development administrator");
        EnsureSuccess(await users.AddToRolesAsync(user, [RoleConstants.ADMIN, RoleConstants.USER]),
            "Assign development administrator roles");
    }

    private static void EnsureSuccess(IdentityResult result, string operation)
    {
        if (!result.Succeeded)
            throw new InvalidOperationException($"{operation} failed: {string.Join(", ", result.Errors.Select(e => e.Code))}");
    }
}
