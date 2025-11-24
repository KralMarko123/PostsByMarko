namespace PostsByMarko.Host.Data.Entities
{
    public class ChatUser
    {
        public Guid ChatId { get; set; }
        public Chat Chat { get; set; } = default!;
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;
    }
}
