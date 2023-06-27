namespace PostsByMarko.Host.Data.Models
{
    public class RequestUser
    {
        public string Username { get; set; } = string.Empty;
        public string UserId { get; set; } = Guid.NewGuid().ToString();
        public List<string> UserRoles { get; set; } = new List<string>();
    }
}
