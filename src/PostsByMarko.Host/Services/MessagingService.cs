using PostsByMarko.Host.Builders;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Host.Repos.Messaging;

namespace PostsByMarko.Host.Services
{
    public class MessagingService : IMessagingService
    {
        private readonly IMessagingRepository messagingRepository;
        
        public MessagingService(IMessagingRepository messagingRepository)
        {
            this.messagingRepository = messagingRepository;
        }

        public async Task<RequestResult> StartChatAsync(string[] participantIds)
        {
            var existingChat = await messagingRepository.GetChatByParticipantIdsAsync(participantIds);

            if (existingChat != null)
            {
                return new RequestResultBuilder()
                    .Ok()
                    .WithPayload(existingChat)
                    .Build();
            }
            
            var createdChat = await messagingRepository.CreateChatAsync(new Chat([.. participantIds]));

            return new RequestResultBuilder()
                .Ok()
                .WithPayload(createdChat)
                .Build();
        }

        public async Task<RequestResult> SendMessageAsync(int chatId, string senderId, string content)
        {
            var forbiddenRequest = new RequestResultBuilder()
                .Forbidden()
                .WithMessage("You are not a participant of this chat")
                .Build();
            var notFoundRequest = new RequestResultBuilder()
                .NotFound()
                .WithMessage($"Chat with Id: {chatId} was not found")
                .Build();
            var chat = await messagingRepository.GetChatByIdAsync(chatId);

            if (chat == null)
                return notFoundRequest;

            if (!chat.ParticipantIds.Contains(senderId))
                return forbiddenRequest;

            var createdMessage = await messagingRepository.CreateMessageAsync(new Message(chatId, senderId, content));

            return new RequestResultBuilder()
                .Ok()
                .WithPayload(createdMessage)
                .Build();
        }
    }
}
