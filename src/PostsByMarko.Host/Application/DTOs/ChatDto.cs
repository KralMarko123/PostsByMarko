namespace PostsByMarko.Host.Application.DTOs
{
    public class ChatDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public List<UserDto> Users { get; set; } = new List<UserDto>();
        public List<MessageDto> Messages { get; set; } = new List<MessageDto>();
    }
}
