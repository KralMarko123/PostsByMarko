using AutoMapper;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Exceptions;
using PostsByMarko.Host.Application.Interfaces;
using PostsByMarko.Host.Data.Entities;
using PostsByMarko.Host.Data.Repositories.Messaging;
using PostsByMarko.Host.Data.Repositories.Users;

namespace PostsByMarko.Host.Application.Services
{
    public class MessagingService : IMessagingService
    {
        private readonly IChatRepository chatRepository;
        private readonly IMessageRepository messageRepository;
        private readonly IUserRepository userRepository;
        private readonly ICurrentRequestAccessor currentRequestAccessor;
        private readonly IMapper mapper;

        public MessagingService(IChatRepository chatRepository, IMessageRepository messageRepository, IUserRepository userRepository, ICurrentRequestAccessor currentRequestAccessor, IMapper mapper)
        {
            this.chatRepository = chatRepository;
            this.messageRepository = messageRepository;
            this.userRepository = userRepository;
            this.currentRequestAccessor = currentRequestAccessor;
            this.mapper = mapper;
        }

        public async Task<List<ChatDto>> GetUserChatsAsync(CancellationToken cancellationToken = default)
        {
            var currentUserId = Guid.Parse(currentRequestAccessor.Id);
            var currentUser = await userRepository.GetUserByIdAsync(currentUserId) ?? throw new KeyNotFoundException($"User with Id: {currentUserId} was not found");
            var chats = await chatRepository.GetChatsForUserAsync(currentUser, cancellationToken);

            return mapper.Map<List<ChatDto>>(chats);
        }

        public async Task<ChatDto> StartChatAsync(Guid otherUserId, CancellationToken cancellationToken = default)
        {
            var currentUserId = Guid.Parse(currentRequestAccessor.Id);
            var currentUser = await userRepository.GetUserByIdAsync(currentUserId) ?? throw new KeyNotFoundException($"User with Id: {currentUserId} was not found");
            var otherUser = await userRepository.GetUserByIdAsync(otherUserId) ?? throw new KeyNotFoundException($"User with Id: {otherUserId} was not found");
            var existingChat = await chatRepository.GetChatByUserIdsAsync([currentUser.Id, otherUserId], cancellationToken);

            if(existingChat != null)
            {
                return mapper.Map<ChatDto>(existingChat);
            }

            var newChat = new Chat
            {
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Messages = new List<Message>(),
                ChatUsers = new List<ChatUser>
                {
                    new ChatUser { UserId = currentUser.Id, User = currentUser },
                    new ChatUser { UserId = otherUser.Id, User = otherUser }
                }
            };

            var createdChat = await chatRepository.AddChatAsync(newChat, cancellationToken);

            await chatRepository.SaveChangesAsync(cancellationToken);

            return mapper.Map<ChatDto>(createdChat);
        }

        public async Task<MessageDto> SendMessageAsync(MessageDto messageDto, CancellationToken cancellationToken = default)
        {
            var sender = await userRepository.GetUserByIdAsync(messageDto.SenderId) ?? throw new KeyNotFoundException($"Sender with Id: {messageDto.SenderId} was not found");
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
