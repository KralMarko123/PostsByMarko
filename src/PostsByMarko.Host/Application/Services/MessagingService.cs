using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Exceptions;
using PostsByMarko.Host.Application.Hubs;
using PostsByMarko.Host.Application.Hubs.Client;
using PostsByMarko.Host.Application.Interfaces;
using PostsByMarko.Host.Application.Requests;
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
        private readonly IHubContext<MessageHub, IMessageClient> messageHub;

        public MessagingService(IChatRepository chatRepository, IMessageRepository messageRepository,
            IUserRepository userRepository, ICurrentRequestAccessor currentRequestAccessor, IMapper mapper, IHubContext<MessageHub, IMessageClient> messageHub)
        {
            this.chatRepository = chatRepository;
            this.messageRepository = messageRepository;
            this.userRepository = userRepository;
            this.currentRequestAccessor = currentRequestAccessor;
            this.mapper = mapper;
            this.messageHub = messageHub;
        }

        public async Task<List<ChatDto>> GetUserChatsAsync(CancellationToken cancellationToken = default)
        {
            var currentUserId = currentRequestAccessor.Id;
            var currentUser = await userRepository.GetUserByIdAsync(currentUserId, cancellationToken) ?? throw new KeyNotFoundException($"User with Id: {currentUserId} was not found");
            var chats = await chatRepository.GetChatsForUserAsync(currentUser, cancellationToken);

            return mapper.Map<List<ChatDto>>(chats);
        }

        public async Task<ChatDto> StartChatAsync(Guid otherUserId, CancellationToken cancellationToken = default)
        {
            var currentUserId = currentRequestAccessor.Id;
            var currentUser = await userRepository.GetUserByIdAsync(currentUserId, cancellationToken) ?? throw new KeyNotFoundException($"User with Id: {currentUserId} was not found");
            var otherUser = await userRepository.GetUserByIdAsync(otherUserId, cancellationToken) ?? throw new KeyNotFoundException($"User with Id: {otherUserId} was not found");
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

            var chatDto = mapper.Map<ChatDto>(createdChat);
            var chatUserIds = chatDto.Users.Select(u => u.Id.ToString());

            await messageHub.Clients.Users(chatUserIds!).ChatCreated(chatDto);

            return chatDto;
        }

        public async Task<MessageDto> SendMessageAsync(SendMessageRequest request, CancellationToken cancellationToken = default)
        {
            var currentUserId = currentRequestAccessor.Id;
            var chat = await chatRepository.GetChatByIdAsync(request.ChatId, cancellationToken) ?? throw new KeyNotFoundException($"Chat with Id: {request.ChatId} was not found");
            var chatUserIds = chat.ChatUsers.Select(c => c.UserId);

            if (!chatUserIds.Contains(currentUserId))
            {
                throw new AuthException("Current user is not a member of the chat.");
            }

            var newMessage = new Message
            {
                ChatId = chat.Id,
                SenderId = currentUserId,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow
            };
            var createdMessage = await messageRepository.AddMessageAsync(newMessage, cancellationToken);

            await messageRepository.SaveChangesAsync(cancellationToken);

            chat.UpdatedAt = DateTime.UtcNow;

            await chatRepository.SaveChangesAsync(cancellationToken);

            var result = mapper.Map<MessageDto>(createdMessage);

            await messageHub.Clients.Users(chat.ChatUsers.Select(c => c.UserId.ToString())).MessageSent(result);

            return result;
        }
    }
}
