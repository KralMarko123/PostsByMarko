using Bogus;
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
            var admins = AppConstants.ADMINS;
            var users = AppConstants.USERS;

            // seed roles
            builder.Entity<IdentityRole>().HasData(appRoles);

            // seed users
            builder.Entity<User>().HasData(admins);
            builder.Entity<User>().HasData(users);

            // set password hashes

            foreach (var user in admins.Concat(users)) 
            {
                user.PasswordHash = passwordHasher.HashPassword(user, "@Marko123");
            }

            // seed userRoles
            List<IdentityUserRole<string>> userRoles = new List<IdentityUserRole<string>>();

            foreach (var user in admins.Concat(users))
            {
                if(admins.Contains(user)) userRoles.Add(new IdentityUserRole<string> { UserId = user.Id, RoleId = appRoles[0].Id });

                userRoles.Add(new IdentityUserRole<string> { UserId = user.Id, RoleId = appRoles[1].Id });
            }

            builder.Entity<IdentityUserRole<string>>().HasData(userRoles);

            // generate random data for local development
            if(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                GenerateRandomData(builder, passwordHasher, 5);
            }
        }

        public static void GenerateRandomData(ModelBuilder builder, PasswordHasher<User> hasher, int numberOfUsers)
        {
            var halfOfMonthHasGoneBy = DateTime.UtcNow.Day >= 15;

            var userFaker = new Faker<User>()
                .CustomInstantiator(f => new User(
                    f.Internet.Email(),
                    f.Name.FirstName(),
                    f.Name.LastName(),
                    true,
                    halfOfMonthHasGoneBy ? f.Date.Recent(30, DateTime.UtcNow) : f.Date.Soon(30, DateTime.UtcNow)
                ));

            var postFaker = new Faker<Post>()
                .CustomInstantiator(f => new Post
                {
                    Title = f.Commerce.ProductName(),
                    Content = f.Rant.Review(f.Commerce.ProductName()),
                    CreatedDate = halfOfMonthHasGoneBy ? f.Date.Recent(30, DateTime.UtcNow) : f.Date.Soon(30, DateTime.UtcNow),
                    LastUpdatedDate = halfOfMonthHasGoneBy ? f.Date.Recent(30, DateTime.UtcNow) : f.Date.Soon(30, DateTime.UtcNow),
                    IsHidden = f.Random.Bool(0.25f)
                });


            var fakeUsers = userFaker.Generate(numberOfUsers);
            var userRoles = new List<IdentityUserRole<string>>();
            var fakePosts = new List<Post>();

            fakeUsers.ForEach(u =>
            {
                u.PasswordHash = hasher.HashPassword(u, "@Marko123");
                userRoles.Add(new IdentityUserRole<string> { UserId = u.Id, RoleId = AppConstants.APP_ROLES[1].Id });
                
                var postsToAdd = postFaker.Generate(new Random().Next(1, 3));
                postsToAdd.ForEach(p => p.AuthorId = u.Id);
                fakePosts.AddRange(postsToAdd);
            });

            builder.Entity<User>().HasData(fakeUsers);
            builder.Entity<IdentityUserRole<string>>().HasData(userRoles);
            builder.Entity<Post>().HasData(fakePosts);
        }
    }
}
