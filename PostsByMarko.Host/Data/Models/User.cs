using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostsByMarko.Host.Data.Models
{
    public class User : IdentityUser
    {
        public override string Email { get; set; }
        public override bool EmailConfirmed { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        [NotMapped]
        public List<string> PostIds { get; set; } = new List<string>();

        public User(string email)
        {
            Email = email;
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
