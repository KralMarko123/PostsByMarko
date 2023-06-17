namespace PostsByMarko.Host.Data.Models.Responses
{
    public class PostDetailsResponse
    {
        public Post? Post { get; set; }
        public string? AuthorFirstName { get; set; }
        public string? AuthorLastName { get; set; }
    }
}
