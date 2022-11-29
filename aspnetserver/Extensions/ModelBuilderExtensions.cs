using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static aspnetserver.Constants.AppConstants;

namespace aspnetserver.Data.Models
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder builder)
        {
            var passwordHasher = new PasswordHasher<User>();

            //seed roles
            builder.Entity<IdentityRole>().HasData(appRoles);

            //seed users
            builder.Entity<User>().HasData(appUsers);

            //seed userRoles
            List<IdentityUserRole<string>> userRoles = new List<IdentityUserRole<string>>();

            appUsers[0].PasswordHash = passwordHasher.HashPassword(appUsers[0], "@Marko123");
            appUsers[1].PasswordHash = passwordHasher.HashPassword(appUsers[1], "Test123");

            appUsers.ForEach(u =>
            {
                userRoles.Add(new IdentityUserRole<string>
                {
                    UserId = u.Id,
                    RoleId = appRoles.First(q => q.Name == "Admin").Id
                });

                userRoles.Add(new IdentityUserRole<string>
                {
                    UserId = u.Id,
                    RoleId = appRoles.First(q => q.Name == "Editor").Id
                });
            });

            builder.Entity<IdentityUserRole<string>>().HasData(userRoles);
        }
    }
}
