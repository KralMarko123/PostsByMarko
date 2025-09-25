namespace PostsByMarko.Host.Data.Models.Responses
{
    public class LoginResponse
    {
        public string? Token { get; set; }
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public List<string>? Roles { get; set; }
    }
}
