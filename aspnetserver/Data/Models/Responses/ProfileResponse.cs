namespace aspnetserver.Data.Models.Responses
{
    public class ProfileResponse
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<string> Roles { get; set; }
    }
}
