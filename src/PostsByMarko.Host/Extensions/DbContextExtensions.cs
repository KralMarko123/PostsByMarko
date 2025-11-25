using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PostsByMarko.Host.Application.Constants;
using PostsByMarko.Host.Data;
using PostsByMarko.Host.Data.Entities;

namespace PostsByMarko.Host.Extensions
{
    public static class DbContextExtensions
    {
        public static async Task Seed(this AppDbContext appDbContext)
        {
            var passwordHasher = new PasswordHasher<User>();
            var appRoles = AppConstants.appRoles;
            var admins = AppConstants.admins;
            var users = AppConstants.appUsers;
            var allUsers = admins.Concat(users);

            // seed roles
            await appDbContext.Roles.AddRangeAsync(appRoles);

            // seed users
            await appDbContext.Users.AddRangeAsync(allUsers);

            // set password hashes
            foreach (var user in allUsers)
            {
                user.Id = Guid.NewGuid();
                user.PasswordHash = passwordHasher.HashPassword(user, "@Marko123");
            }

            // seed userRoles
            List<IdentityUserRole<Guid>> userRoles = [];

            foreach (var user in allUsers)
            {
                if (admins.Contains(user))
                {
                    userRoles.Add(new IdentityUserRole<Guid> { UserId = user.Id, RoleId = appRoles[0].Id });
                }

                userRoles.Add(new IdentityUserRole<Guid> {UserId = user.Id, RoleId = appRoles[1].Id });
            }

            await appDbContext.UserRoles.AddRangeAsync(userRoles);
            await appDbContext.SaveChangesAsync();

            // generate random data for local development & tests
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (environment == "Development" || environment == "Test")
            {
                await GenerateRandomData(appDbContext, passwordHasher, 5);
            }
        }

        public static async Task GenerateRandomData(AppDbContext appDbContext, PasswordHasher<User> hasher, int numberOfUsers)
        {
            var halfOfMonthHasGoneBy = DateTime.UtcNow.Day >= 15;

            var userFaker = new Faker<User>()
                .CustomInstantiator(f => new User
                {
                    Email = f.Internet.Email(),
                    FirstName = f.Name.FirstName(),
                    LastName = f.Name.LastName(),
                    EmailConfirmed = true,
                });

            var postFaker = new Faker<Post>()
                .CustomInstantiator(f => new Post
                {
                    Title = f.Commerce.ProductName(),
                    Content = f.Rant.Review(f.Commerce.ProductName()),
                    CreatedDate = halfOfMonthHasGoneBy ? f.Date.Recent(30, DateTime.UtcNow) : f.Date.Soon(30, DateTime.UtcNow),
                    LastUpdatedDate = halfOfMonthHasGoneBy ? f.Date.Recent(30, DateTime.UtcNow) : f.Date.Soon(30, DateTime.UtcNow),
                    Hidden = f.Random.Bool(0.25f)
                });


            var fakeUsers = userFaker.Generate(numberOfUsers);
            var userRoles = new List<IdentityUserRole<Guid>>();
            var fakePosts = new List<Post>();

            fakeUsers.ForEach(u =>
            {
                u.Id = Guid.NewGuid();
                u.PasswordHash = hasher.HashPassword(u, "@Marko123");
                userRoles.Add(new IdentityUserRole<Guid> { UserId = u.Id, RoleId = AppConstants.appRoles[1].Id });

                var postsToAdd = postFaker.Generate(new Random().Next(1, 3));

                postsToAdd.ForEach(p =>
                {
                    p.Author = u;
                    p.AuthorId = u.Id;
                });
                u.Posts = postsToAdd;
                fakePosts.AddRange(postsToAdd);
            });

            await appDbContext.Users.AddRangeAsync(fakeUsers);
            await appDbContext.UserRoles.AddRangeAsync(userRoles);
            await appDbContext.Posts.AddRangeAsync(fakePosts);
            await appDbContext.SaveChangesAsync();
        }
    }
}
