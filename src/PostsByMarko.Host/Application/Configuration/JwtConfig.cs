namespace PostsByMarko.Host.Application.Configuration
{
    public class JwtConfig
    {
        public List<string> ValidIssuers { get; set; } = new List<string>();
        public List<string> ValidAudiences { get; set; } = new List<string>();
        public string Secret { get; set; } = string.Empty;
        public int ExpiresIn { get; set; } = 100;
    }
}
