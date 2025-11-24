using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostsByMarko.Host.Data.Entities
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid ChatId { get; set; }
        public Chat Chat { get; set; } = default!;
        public Guid SenderId { get; set; }
        public User Sender { get; set; } = default!;

        public Message()
        {

        }
    }
}
