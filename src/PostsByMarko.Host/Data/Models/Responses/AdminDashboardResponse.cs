namespace PostsByMarko.Host.Data.Models.Responses
{
    public class AdminDashboardResponse
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public int NumberOfPosts { get; set; }
        public DateTime? LastPostedAt { get; set; }
        public List<string> Roles { get; set; }
    }
}
