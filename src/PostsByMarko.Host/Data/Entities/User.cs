using Microsoft.AspNetCore.Identity;

namespace PostsByMarko.Host.Data.Entities
{
    public class User : IdentityUser<Guid>
    {
        public override string Email { get; set; } = string.Empty;
        public override bool EmailConfirmed { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        // Navigation properties
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<ChatUser> ChatUsers { get; set; } = new List<ChatUser>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();

        public User() { }

        public User(string email)
        {
            Email = email;
            NormalizedEmail = email.ToUpper();
            UserName = email;
            NormalizedUserName = email.ToUpper();
        }

        public User(string email, string firstName, string lastName)
        {
            Email = email;
            NormalizedEmail = email.ToUpper();
            UserName = email;
            NormalizedUserName = email.ToUpper();
            FirstName = firstName;
            LastName = lastName;
            EmailConfirmed = false;
        }

        public User(string email, string firstName, string lastName, bool emailConfirmed)
        {
            Email = email;
            NormalizedEmail = email.ToUpper();
            UserName = email;
            NormalizedUserName = email.ToUpper();
            FirstName = firstName;
            LastName = lastName;
            EmailConfirmed = emailConfirmed;
        }
    }
}
