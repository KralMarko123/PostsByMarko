namespace PostsByMarko.Host.Application.DTOs
{
    public class PostDto
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public UserDto Author { get; set; } = default!;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdatedDate { get; set; } = DateTime.UtcNow;
        public bool Hidden { get; set; } = false;
    }
}
