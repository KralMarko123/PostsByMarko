using System.ComponentModel.DataAnnotations;

namespace PostsByMarko.Host.Application.Requests
{
    public class SendMessageRequest
    {
        public Guid ChatId { get; set; }
        [Required, StringLength(4000)]
        public string Content { get; set; } = string.Empty;
    }
}
