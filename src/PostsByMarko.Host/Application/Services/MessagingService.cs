using AutoMapper;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Exceptions;
using PostsByMarko.Host.Application.Interfaces;
using PostsByMarko.Host.Data.Entities;
using PostsByMarko.Host.Data.Repositories.Messaging;

namespace PostsByMarko.Host.Application.Services
{
    public class MessagingService : IMessagingService
    {
        private readonly IChatRepository chatRepository;
        private readonly IMessageRepository messageRepository;
        private readonly IUsersService usersService;
        private readonly IMapper mapper;

        public MessagingService(IChatRepository chatRepository, IMessageRepository messageRepository, IUsersService usersService, IMapper mapper)
        {
            this.chatRepository = chatRepository;
            this.messageRepository = messageRepository;
            this.usersService = usersService;
            this.mapper = mapper;
        }

        public async Task<List<ChatDto>> GetUserChatsAsync(CancellationToken cancellationToken = default)
        {
            var currentUser = await usersService.GetCurrentUserAsync();
            var chats = await chatRepository.GetChatsForUserAsync(currentUser, cancellationToken);
            var result = mapper.Map<List<ChatDto>>(chats);

            return result;
        }

        public async Task<ChatDto> StartChatAsync(Guid otherUserId, CancellationToken cancellationToken = default)
        {
            var currentUser = await usersService.GetCurrentUserAsync();

            try
            {
                var existingChat = await chatRepository.GetChatByUserIdsAsync([currentUser.Id, otherUserId], cancellationToken);

                return mapper.Map<ChatDto>(existingChat);
            }
            catch (KeyNotFoundException)
            {
                var newChat = new Chat
                {
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Messages = new List<Message>(),
                    ChatUsers = new List<ChatUser>
                    {
                        new ChatUser { UserId = currentUser.Id },
                        new ChatUser { UserId = otherUserId }
                    }
                };

                var createdChat = await chatRepository.AddChatAsync(newChat, cancellationToken);

                await chatRepository.SaveChangesAsync(cancellationToken);

                return mapper.Map<ChatDto>(createdChat);
            }
        }

        public async Task<MessageDto> SendMessageAsync(MessageDto messageDto, CancellationToken cancellationToken = default)
        {
            var sender = await usersService.GetUserByIdAsync(messageDto.SenderId);
            var chat = await chatRepository.GetChatByIdAsync(messageDto.ChatId, cancellationToken) ?? throw new KeyNotFoundException($"Chat with Id: {messageDto.ChatId} was not found");

            if (!chat.ChatUsers.Select(c => c.UserId).Contains(messageDto.SenderId))
            {
                throw new AuthException("Sender is not a member of the chat.");
            }

            var newMessage = mapper.Map<Message>(messageDto);
            var createdMessage = await messageRepository.AddMessageAsync(newMessage, cancellationToken);

            await messageRepository.SaveChangesAsync(cancellationToken);
            
            return mapper.Map<MessageDto>(createdMessage);
        }
    }
}
