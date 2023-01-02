using Microsoft.EntityFrameworkCore;
using aspnetserver.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace aspnetserver.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public DbSet<Post>? Posts { get; set; }

        public AppDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Seed();
        }
    }
}
