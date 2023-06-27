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

            //seed roles
            builder.Entity<IdentityRole>().HasData(appRoles);

            //seed users
            builder.Entity<User>().HasData(defaultUsers);

            //seed userRoles
            List<IdentityUserRole<string>> userRoles = new List<IdentityUserRole<string>>();

            defaultUsers[0].PasswordHash = passwordHasher.HashPassword(defaultUsers[0], "@PostsByMarko123");
            defaultUsers[1].PasswordHash = passwordHasher.HashPassword(defaultUsers[1], "@PostsByMarko123");

            defaultUsers.ForEach(u =>
            {
                userRoles.Add(new IdentityUserRole<string>
                {
                    UserId = u.Id,
                    RoleId = appRoles.First(q => q.Name == "Admin").Id
                });

                userRoles.Add(new IdentityUserRole<string>
                {
                    UserId = u.Id,
                    RoleId = appRoles.First(q => q.Name == "User").Id
                });
            });

            builder.Entity<IdentityUserRole<string>>().HasData(userRoles);
        }
    }
}
