using Microsoft.AspNetCore.Identity;

namespace PostsByMarko.Host.Data.Entities
{
    public class User : IdentityUser<Guid>
    {
        public override string? Email { get; set; }
        public override bool EmailConfirmed { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        // Navigation properties
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<ChatUser> ChatUsers { get; set; } = new List<ChatUser>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();

        public User() { }
    }
}
