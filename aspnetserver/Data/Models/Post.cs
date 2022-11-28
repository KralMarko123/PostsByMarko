using System.ComponentModel.DataAnnotations;

namespace aspnetserver.Data.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(100000)]
        public string Content { get; set; } = string.Empty;

        public string AuthorId { get; set; }
        public User? Author { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
    }
}
