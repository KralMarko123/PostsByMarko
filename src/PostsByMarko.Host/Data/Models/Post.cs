using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostsByMarko.Host.Data.Models
{
    public class Post
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string AuthorId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdatedDate { get; set; } = DateTime.UtcNow;
        public bool IsHidden { get; set; } = false;

        public Post(string title, string content)
        {
            Title = title;
            Content = content;
        }

        public Post(string title, string content, string authorId, DateTime createdDate, DateTime updatedDate, bool isHidden)
        {
            Title = title;
            Content = content;
            AuthorId = authorId;
            CreatedDate = createdDate;
            LastUpdatedDate = updatedDate;
            IsHidden = isHidden;
        }

        public Post() { }
    }
}
