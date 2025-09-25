namespace PostsByMarko.Host.Data.Models
{
    public class RequestUser
    {
        public string Email { get; set; } = string.Empty;
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public List<string> Roles { get; set; } = new List<string>();
    }
}
