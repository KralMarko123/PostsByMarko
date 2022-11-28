using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace aspnetserver.Data.Models
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder builder)
        {
            //seed roles
            List<IdentityRole> roles = new List<IdentityRole>()
            {
                new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Name = "Editor", NormalizedName = "EDITOR" },
            };

            builder.Entity<IdentityRole>().HasData(roles);

            //seed users
            var passwordHasher = new PasswordHasher<User>();

            List<User> users = new List<User>()
            {
                 new User {
                    FirstName ="Marko",
                    LastName = "Markovikj",
                    UserName = "kralmarko123@gmail.com",
                    NormalizedUserName = "KRALMARKO123@GMAIL.COM",
                    Email = "kralmarko123@gmail.com",
                    NormalizedEmail = "KRALMARKO123@GMAIL.COM",
                    Posts = new List<Post>()
                },

                new User {
                    FirstName = "Test",
                    LastName = "Testerson",
                    UserName = "test@test.com",
                    NormalizedUserName = "TEST@TEST.COM",
                    Email = "test@test.com",
                    NormalizedEmail = "TEST@TEST.COM",
                    Posts = new List<Post>()
                }
            };

            builder.Entity<User>().HasData(users);

            //seed userRoles
            List<IdentityUserRole<string>> userRoles = new List<IdentityUserRole<string>>();

            users[0].PasswordHash = passwordHasher.HashPassword(users[0], "@Marko123");
            users[1].PasswordHash = passwordHasher.HashPassword(users[1], "Test123");

            users.ForEach(u =>
            {
                userRoles.Add(new IdentityUserRole<string>
                {
                    UserId = u.Id,
                    RoleId = roles.First(q => q.Name == "Admin").Id
                });

                userRoles.Add(new IdentityUserRole<string>
                {
                    UserId = u.Id,
                    RoleId = roles.First(q => q.Name == "Editor").Id
                });
            });

            builder.Entity<IdentityUserRole<string>>().HasData(userRoles);
        }
    }
}
