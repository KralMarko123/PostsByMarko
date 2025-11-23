namespace PostsByMarko.Host.Application.Responses
{
    public class AdminDashboardResponse
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public int NumberOfPosts { get; set; }
        public DateTime? LastPostedAt { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}
