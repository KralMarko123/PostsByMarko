using Microsoft.EntityFrameworkCore;
using aspnetserver.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Text.Json;

namespace aspnetserver.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public DbSet<Post> Posts { get; set; }

        public AppDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>().Property(p => p.AllowedUsers)
                .HasConversion(p => JsonSerializer.Serialize(p, (JsonSerializerOptions)default),
                               p => JsonSerializer.Deserialize<List<string>>(p, (JsonSerializerOptions)default));

            base.OnModelCreating(modelBuilder);
            modelBuilder.Seed();
        }
    }
}
