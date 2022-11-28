using Microsoft.AspNetCore.Identity;

namespace aspnetserver.Data.Models
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public List<Post>? Posts { get; set; }
    }
}
