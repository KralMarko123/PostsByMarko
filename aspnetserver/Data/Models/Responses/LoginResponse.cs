namespace aspnetserver.Data.Models.Responses
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public ProfileResponse Profile { get; set; }
    }
}
