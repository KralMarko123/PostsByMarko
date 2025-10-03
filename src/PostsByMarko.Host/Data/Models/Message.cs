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
        public Message(string content, string senderId, int chatId)
        {
            Content = content;
            SenderId = senderId;
            ChatId = chatId;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
