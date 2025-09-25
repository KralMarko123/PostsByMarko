using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
            modelBuilder.Entity<User>()
                .HasMany(u => u.Posts)
                .WithOne()
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
            modelBuilder.Seed();
        }
    }
}
