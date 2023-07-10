using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostsByMarko.Host.Data.Models
{
    public class Post
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string? AuthorId { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdatedDate { get; set; } = DateTime.UtcNow;
        public bool IsHidden { get; set; } = false;

        [NotMapped]
        public List<string> AllowedUsers { get; set; } = new List<string>();

        public Post(string title, string content)
        {
            Title = title;
            Content = content;
        }

        public Post() { }
    }
}
