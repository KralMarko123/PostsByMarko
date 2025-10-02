using Microsoft.AspNetCore.Identity;
using PostsByMarko.Host.Data.Models;

namespace PostsByMarko.Host.Constants
{
    public static class AppConstants
    {

        public static List<IdentityRole> APP_ROLES = new List<IdentityRole>()
        {
                new IdentityRole { Name = RoleConstants.ADMIN, NormalizedName = RoleConstants.ADMIN.ToUpper() },
                new IdentityRole { Name = RoleConstants.USER, NormalizedName = RoleConstants.USER.ToUpper() }
        };


        public static List<User> DEFAULT_USERS = new List<User>()
        {
                new User("kralmarko123@gmail.com", "Marko", "Markovikj", true, DateTime.UtcNow),
                new User("test@test.com", "Test", "User", true, DateTime.UtcNow),
        };
    }
}
