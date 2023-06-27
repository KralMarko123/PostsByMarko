using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Text.Json;
using PostsByMarko.Host.Extensions;
using PostsByMarko.Host.Data.Models;

namespace PostsByMarko.Host.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public DbSet<Post> Posts { get; set; }

        public AppDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>().Property(p => p.AllowedUsers)
                .HasConversion(p => JsonSerializer.Serialize(p, (JsonSerializerOptions)default!),
                               p => JsonSerializer.Deserialize<List<string>>(p, (JsonSerializerOptions)default!)!);

            base.OnModelCreating(modelBuilder);
            modelBuilder.Seed();
        }
    }
}
