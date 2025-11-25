using Microsoft.AspNetCore.Identity;
using PostsByMarko.Host.Data.Entities;

namespace PostsByMarko.Host.Application.Constants
{
    public static class AppConstants
    {

        public static List<IdentityRole<Guid>> appRoles =
        [
                new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = RoleConstants.ADMIN, NormalizedName = RoleConstants.ADMIN.ToUpper() },
                new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = RoleConstants.USER, NormalizedName = RoleConstants.USER.ToUpper() }
        ];


        public static List<User> admins =
        [
                new User
                {
                    Email = "kralmarko123@gmail.com",
                    NormalizedEmail = "KRALMARKO123@gmail.com",
                    UserName = "kralmarko123@gmail.com",
                    NormalizedUserName = "KRALMARKO123@gmail.com",
                    FirstName = "Marko",
                    LastName = "Markovikj",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new User
                {
                    Email = "testAdmin@test.com",
                    NormalizedEmail = "TESTADMIN@TEST.COM",
                    UserName = "testAdmin@test.com",
                    NormalizedUserName = "TESTADMIN@TEST.COM",
                    FirstName = "Test",
                    LastName = "Admin",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                }
        ];

        public static List<User> appUsers =
        [
                new User
                { 
                    Email = "test@test.com",
                    NormalizedEmail = "TEST@TEST.COM",
                    UserName = "test@test.com",
                    NormalizedUserName = "TEST@TEST.COM",
                    FirstName = "Test", 
                    LastName = "User",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString() 
                },
                new User
                {
                    Email = "user@userson.com",
                    NormalizedEmail = "USER@USERSON.COM",
                    UserName = "user@userson.com",
                    NormalizedUserName = "USER@USERSON.COM",
                    FirstName = "User",
                    LastName = "Userson",
                    EmailConfirmed = false,
                    SecurityStamp = Guid.NewGuid().ToString()
                }
        ];
    }
}
