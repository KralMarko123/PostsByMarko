namespace PostsByMarko.Host.Application.DTOs
{
    public class MessageDto
    {
        public string? Content { get; set; }
        public Guid SenderId { get; set; }
        public Guid ChatId { get; set; }
    }
}
