using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Builders;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Host.Data.Models.Dtos;
using PostsByMarko.Host.Data.Repos.Messaging;

namespace PostsByMarko.Host.Application.Services
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
                existingChat.Messages = await messagingRepository.GetChatMessagesAsync(existingChat);
                
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

        public async Task<RequestResult> SendMessageAsync(MessageDto messageDto)
        {
            var forbiddenRequest = new RequestResultBuilder()
                .Forbidden()
                .WithMessage("You are not a participant of this chat")
                .Build();
            var notFoundRequest = new RequestResultBuilder()
                .NotFound()
                .WithMessage($"Chat with Id: {messageDto.ChatId} was not found")
                .Build();
            var badRequest = new RequestResultBuilder()
                .BadRequest()
                .WithMessage("Error during chat update")
                .Build();

            var chat = await messagingRepository.GetChatByIdAsync(messageDto.ChatId);

            if (chat == null)
                return notFoundRequest;

            if (!chat.ParticipantIds.Contains(messageDto.SenderId))
                return forbiddenRequest;

            var createdMessage = await messagingRepository.CreateMessageAsync(new Message(messageDto));

            chat.Messages.Add(createdMessage);
            chat.UpdatedAt = DateTime.UtcNow;

            var chatUpdatedSuccessfully = await messagingRepository.UpdateChatAsync(chat);

            if (!chatUpdatedSuccessfully)
                return badRequest;

            return new RequestResultBuilder()
                .Ok()
                .WithPayload(createdMessage)
                .Build();
        }

        public async Task<RequestResult> GetUserChatsAsync(string userId)
        {
            var chats = await messagingRepository.GetUserChatsAsync(userId);

            if(chats != null)
            {
                foreach (var chat in chats)
                {
                    chat.Messages = await messagingRepository.GetChatMessagesAsync(chat);
                }
            }

            return new RequestResultBuilder()
                .Ok()
                .WithPayload(chats)
                .Build();
        }
    }
}
