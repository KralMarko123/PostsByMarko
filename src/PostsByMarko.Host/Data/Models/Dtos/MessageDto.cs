namespace PostsByMarko.Host.Data.Models.Dtos
{
    public class MessageDto
    {
        public string Content { get; set; }
        public string SenderId { get; set; }
        public int ChatId { get; set; }

        public MessageDto() { }

        public MessageDto(int chatId, string senderId, string content)
        {
            ChatId = chatId;
            SenderId = senderId;
            Content = content;
        }
    }
}
