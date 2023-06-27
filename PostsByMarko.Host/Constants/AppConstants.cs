using Microsoft.AspNetCore.Identity;
using PostsByMarko.Host.Data.Models;

namespace PostsByMarko.Host.Constants
{
    public static class AppConstants
    {

        public static List<IdentityRole> APP_ROLES = new List<IdentityRole>()
        {
                new IdentityRole { Name = RoleConstants.ADMIN },
                new IdentityRole { Name = RoleConstants.USER }
        };


        public static List<User> DEFAULT_USERS = new List<User>()
        {
                 new User
                 {
                    FirstName ="Marko",
                    LastName = "Markovikj",
                    UserName = "kralmarko123@gmail.com",
                    NormalizedUserName = "KRALMARKO123@GMAIL.COM",
                    Email = "kralmarko123@gmail.com",
                    NormalizedEmail = "KRALMARKO123@GMAIL.COM",
                    EmailConfirmed = true,
                    Posts = new List<Post>(),
                 },

                 new User
                 {
                    FirstName = "Test",
                    LastName = "User",
                    UserName = "test_user",
                    NormalizedUserName = "TEST_USER",
                    Email = "test_user@test.com",
                    NormalizedEmail = "TEST_USER@TEST.COM",
                    EmailConfirmed = true,
                    Posts = new List<Post>(),
                 }
        };
    }
}
