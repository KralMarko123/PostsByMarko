using Microsoft.AspNetCore.Identity;

namespace PostsByMarko.Host.Data.Models
{
    public class User : IdentityUser
    {
        public override string Email { get; set; }
        public override bool EmailConfirmed { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public byte[]? ProfilePicture { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Post> Posts { get; set; } = new List<Post>();

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

        public User(string email, string firstName, string lastName, bool emailConfirmed, DateTime createdAt)
        {
            Email = email;
            NormalizedEmail = email.ToUpper();
            UserName = email;
            NormalizedUserName = email.ToUpper();
            FirstName = firstName;
            LastName = lastName;
            EmailConfirmed = emailConfirmed;
            CreatedAt = createdAt;
        }
    }
}
