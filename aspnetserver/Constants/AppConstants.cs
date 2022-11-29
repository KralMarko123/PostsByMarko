using aspnetserver.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace aspnetserver.Constants
{
    public static class AppConstants
    {

        public static List<IdentityRole> appRoles = new List<IdentityRole>()
        {
                new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Name = "Editor", NormalizedName = "EDITOR" },
        };


        public static List<User> appUsers = new List<User>()
        {
                 new User
                 {
                    FirstName ="Marko",
                    LastName = "Markovikj",
                    UserName = "kralmarko123@gmail.com",
                    NormalizedUserName = "KRALMARKO123@GMAIL.COM",
                    Email = "kralmarko123@gmail.com",
                    NormalizedEmail = "KRALMARKO123@GMAIL.COM",
                    Posts = new List<Post>(),
                 },

                 new User
                 {
                    FirstName = "Test",
                    LastName = "Testerson",
                    UserName = "test@test.com",
                    NormalizedUserName = "TEST@TEST.COM",
                    Email = "test@test.com",
                    NormalizedEmail = "TEST@TEST.COM",
                    Posts = new List<Post>(),
                 }
        };
    }
}
