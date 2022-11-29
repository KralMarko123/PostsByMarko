namespace aspnetserver.Data.Models.Responses
{
    public class PostResponse
    {
        public Post Post { get; set; }
        public UserResponse Author { get; set; }
    }
}
