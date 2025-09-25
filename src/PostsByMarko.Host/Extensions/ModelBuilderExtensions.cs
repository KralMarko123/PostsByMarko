using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PostsByMarko.Host.Constants;
using PostsByMarko.Host.Data.Models;

namespace PostsByMarko.Host.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder builder)
        {
            var passwordHasher = new PasswordHasher<User>();
            var appRoles = AppConstants.APP_ROLES;
            var defaultUsers = AppConstants.DEFAULT_USERS;

            // seed roles
            builder.Entity<IdentityRole>().HasData(appRoles);

            // seed users
            builder.Entity<User>().HasData(defaultUsers);

            // set password hashes
            defaultUsers[0].PasswordHash = passwordHasher.HashPassword(defaultUsers[0], "@Marko123");
            defaultUsers[1].PasswordHash = passwordHasher.HashPassword(defaultUsers[1], "@Marko123");

            // seed userRoles
            List<IdentityUserRole<string>> userRoles = new List<IdentityUserRole<string>>();

            userRoles.Add(new IdentityUserRole<string> { UserId = defaultUsers[0].Id, RoleId = appRoles[0].Id });
            userRoles.Add(new IdentityUserRole<string> { UserId = defaultUsers[0].Id, RoleId = appRoles[1].Id });
            userRoles.Add(new IdentityUserRole<string> { UserId = defaultUsers[1].Id, RoleId = appRoles[1].Id });

            builder.Entity<IdentityUserRole<string>>().HasData(userRoles);
        }
    }
}
