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

        public AppDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SetupKeyGenerationStrategy(modelBuilder);
            SetupRelationshipStrategies(modelBuilder);
            SetupIndexes(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void SetupKeyGenerationStrategy(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            modelBuilder.Entity<Post>()
                .Property(b => b.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            modelBuilder.Entity<Chat>()
                .Property(b => b.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            modelBuilder.Entity<Message>()
                .Property(b => b.Id)
                .HasDefaultValueSql("gen_random_uuid()");
        }

        private void SetupRelationshipStrategies(ModelBuilder modelBuilder)
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

        private void SetupIndexes(ModelBuilder modelBuilder)
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