using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostsByMarko.Host.Data.Models
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string SenderId { get; set; }
        public int ChatId { get; set; }

        public Message()
        {
        }
        public Message(int chatId, string senderId, string content)
        {
            ChatId = chatId;
            SenderId = senderId;
            Content = content;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
