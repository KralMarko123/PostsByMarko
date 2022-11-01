
using Microsoft.EntityFrameworkCore;

namespace aspnetserver.Data
{
    internal sealed class AppDBContext : DbContext
    {
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder) => dbContextOptionsBuilder.UseSqlite("Data Source=./Data/AppDB.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Post[] postsToSeed = new Post[6];

            for (int i = 0; i < postsToSeed.Length; i++)
            {
                postsToSeed[i] = new Post
                {
                    PostId = i + 1,
                    Title = $"Post {i + 1}",
                    Content = $"This is post no. {i + 1}. You are currently seeing its content."
                };
            }

            modelBuilder.Entity<Post>().HasData(postsToSeed);
        }

    }
}
