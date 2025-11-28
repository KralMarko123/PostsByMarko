using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PostsByMarko.Host.Data.Entities;

namespace PostsByMarko.Host.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }

        public readonly IHostEnvironment hostEnvironment;

        public AppDbContext(DbContextOptions dbContextOptions, IHostEnvironment hostEnvironment) : base(dbContextOptions)
        {
            this.hostEnvironment = hostEnvironment;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            SetupKeyGenerationStrategy(builder);
            SetupRelationshipStrategies(builder);
            SetupIndexes(builder);

            base.OnModelCreating(builder);
        }

        private static void SetupKeyGenerationStrategy(ModelBuilder modelBuilder)
        {
            const string pgGuidGenerationFunctionName = "gen_random_uuid()";

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasDefaultValueSql(pgGuidGenerationFunctionName);

            modelBuilder.Entity<Post>()
                .Property(b => b.Id)
                .HasDefaultValueSql(pgGuidGenerationFunctionName);

            modelBuilder.Entity<Chat>()
                .Property(b => b.Id)
                .HasDefaultValueSql(pgGuidGenerationFunctionName);

            modelBuilder.Entity<Message>()
                .Property(b => b.Id)
                .HasDefaultValueSql(pgGuidGenerationFunctionName);
        }

        private static void SetupRelationshipStrategies(ModelBuilder modelBuilder)
        {
            // Post → User (1:N)
            modelBuilder.Entity<Post>()
                .HasOne(p => p.Author)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.AuthorId);

            // ChatUser many-to-many
            modelBuilder.Entity<ChatUser>()
                .HasKey(cu => new { cu.ChatId, cu.UserId });

            modelBuilder.Entity<ChatUser>()
                .HasOne(cu => cu.Chat)
                .WithMany(c => c.ChatUsers)
                .HasForeignKey(cu => cu.ChatId);

            modelBuilder.Entity<ChatUser>()
                .HasOne(cu => cu.User)
                .WithMany(u => u.ChatUsers)
                .HasForeignKey(cu => cu.UserId);

            // Message → Chat (1:N)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatId)
                .OnDelete(DeleteBehavior.Cascade);

            // Message → User (1:N)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static void SetupIndexes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>()
                .HasIndex(p => new { p.AuthorId });

            modelBuilder.Entity<Message>()
                .HasIndex(m => new { m.ChatId, m.CreatedAt });

            modelBuilder.Entity<ChatUser>()
                .HasIndex(cu => cu.UserId);
        }
    }
}