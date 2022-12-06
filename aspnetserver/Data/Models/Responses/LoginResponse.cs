namespace aspnetserver.Data.Models.Responses
{
    public class LoginResponse
    {
        public string? Token { get; set; }
        public string? UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public List<string>? Roles { get; set; }
    }
}
