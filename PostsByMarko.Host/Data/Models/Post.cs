using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostsByMarko.Host.Data.Models
{
    public class Post
    {
        [Key]
        public Guid PostId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(100000)]
        public string Content { get; set; } = string.Empty;

        [ForeignKey("User")]
        public string UserId { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public bool IsHidden { get; set; } = false;

        public List<string> AllowedUsers { get; set; } = new List<string>();
    }
}
