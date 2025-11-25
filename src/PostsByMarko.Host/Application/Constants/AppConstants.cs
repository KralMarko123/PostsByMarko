using Microsoft.AspNetCore.Identity;
using PostsByMarko.Host.Data.Entities;

namespace PostsByMarko.Host.Application.Constants
{
    public static class AppConstants
    {

        public static List<IdentityRole<Guid>> APP_ROLES = new()
        {
                new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = RoleConstants.ADMIN, NormalizedName = RoleConstants.ADMIN.ToUpper() },
                new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = RoleConstants.USER, NormalizedName = RoleConstants.USER.ToUpper() }
        };


        public static List<User> ADMINS = new()
        {
                new User("kralmarko123@gmail.com", "Marko", "Markovikj", true),
                new User("test_admin@test.com", "Test", "Admin", true)
        };

        public static List<User> USERS = new()
        {
                new User("test@test.com", "Test", "User", true),
                new User("user@user.com", "User", "Userson", true)
        };
    }
}
