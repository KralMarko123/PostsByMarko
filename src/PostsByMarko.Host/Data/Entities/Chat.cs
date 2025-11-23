using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostsByMarko.Host.Data.Entities
{
    public class Chat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public List<Guid> ParticipantIds { get; set; } = new List<Guid>();

        public Chat()
        {

        }

        public Chat(List<string> participantIds)
        {
            ParticipantIds = participantIds;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
