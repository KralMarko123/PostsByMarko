using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Moq;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Exceptions;
using PostsByMarko.Host.Application.Hubs;
using PostsByMarko.Host.Application.Hubs.Client;
using PostsByMarko.Host.Application.Interfaces;
using PostsByMarko.Host.Application.Services;
using PostsByMarko.Host.Data.Entities;
using PostsByMarko.Host.Data.Repositories.Messaging;
using PostsByMarko.Host.Data.Repositories.Users;

namespace PostsByMarko.UnitTests
{
    public class MessagingServiceTests
    {
        private readonly MessagingService messagingService;
        private readonly Mock<IChatRepository> chatRepositoryMock = new();
        private readonly Mock<IMessageRepository> messageRepositoryMock = new();
        private readonly Mock<IUserRepository> userRepositoryMock = new();
        private readonly Mock<ICurrentRequestAccessor> currentRequestAccessorMock = new();
        private readonly Mock<IMapper> mapperMock = new();
        private readonly Mock<IHubContext<MessageHub, IMessageClient>> messageHubMock = new();
        private readonly Mock<IMessageClient> messageClientMock = new();

        public MessagingServiceTests()
        {
            messagingService = new MessagingService(chatRepositoryMock.Object,
                messageRepositoryMock.Object,
                userRepositoryMock.Object,
                currentRequestAccessorMock.Object,
                mapperMock.Object,
                messageHubMock.Object);
        }

        [Fact]
        public async Task get_user_chats_should_return_chats_of_user()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "test@test.com" };
            var chats = new List<Chat>
            {
                new Chat { Id = Guid.NewGuid() },
                new Chat { Id = Guid.NewGuid() }
            };
            var chatDtos = new List<ChatDto>
            {
                new ChatDto { Id = chats[0].Id },
                new ChatDto { Id = chats[1].Id }
            };

            currentRequestAccessorMock.Setup(cr => cr.Id).Returns(user.Id);
            userRepositoryMock.Setup(us => us.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            chatRepositoryMock.Setup(cr => cr.GetChatsForUserAsync(user, It.IsAny<CancellationToken>())).ReturnsAsync(chats);
            mapperMock.Setup(m => m.Map<List<ChatDto>>(chats)).Returns(chatDtos);

            // Act
            var result = await messagingService.GetUserChatsAsync(CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(chatDtos);
        }

        [Fact]
        public async Task get_user_chats_should_throw_if_user_was_not_found()
        {
            // Arrange
            var randomId = Guid.NewGuid();

            currentRequestAccessorMock.Setup(cr => cr.Id).Returns(randomId);

            // Act
            var result = async () => await messagingService.GetUserChatsAsync(CancellationToken.None);

            // Assert
            await result.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"User with Id: {randomId} was not found");
        }

        [Fact]
        public async Task start_chat_should_return_existing_chat()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid() };
            var otherUser = new User { Id = Guid.NewGuid() };
            Guid[] userIds = [user.Id, otherUser.Id];
            var existingChat = new Chat { Id = Guid.NewGuid() };
            var chatDto = new ChatDto { Id = existingChat.Id };

            currentRequestAccessorMock.Setup(cr => cr.Id).Returns(user.Id);
            userRepositoryMock.Setup(us => us.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            userRepositoryMock.Setup(us => us.GetUserByIdAsync(otherUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(otherUser);
            chatRepositoryMock.Setup(cr => cr.GetChatByUserIdsAsync(userIds, It.IsAny<CancellationToken>())).ReturnsAsync(existingChat);
            mapperMock.Setup(m => m.Map<ChatDto>(existingChat)).Returns(chatDto);

            // Act
            var result = await messagingService.StartChatAsync(otherUser.Id, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(chatDto);
        }

        [Fact]
        public async Task start_chat_should_create_a_new_chat()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid() };
            var otherUser = new User { Id = Guid.NewGuid() };
            Guid[] userIds = [user.Id, otherUser.Id];
            var newChat = new Chat
            {
                Id = Guid.NewGuid(),    
                ChatUsers = new List<ChatUser>
                {
                    new ChatUser { UserId = user.Id },
                    new ChatUser { UserId = otherUser.Id }
                }
            };
            var chatDto = new ChatDto
            { 
                Id = newChat.Id, Users = new List<UserDto>
                {
                    new UserDto { Id = user.Id },
                    new UserDto { Id = otherUser.Id }
                }
            };

            currentRequestAccessorMock.Setup(cr => cr.Id).Returns(user.Id);
            userRepositoryMock.Setup(us => us.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            userRepositoryMock.Setup(us => us.GetUserByIdAsync(otherUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(otherUser);
            chatRepositoryMock.Setup(cr => cr.AddChatAsync(It.IsAny<Chat>(), It.IsAny<CancellationToken>())).ReturnsAsync(newChat);
            mapperMock.Setup(m => m.Map<ChatDto>(newChat)).Returns(chatDto);
            messageHubMock.Setup(m => m.Clients.Users(It.IsAny<List<string>>())).Returns(messageClientMock.Object);
            messageClientMock.Setup(m => m.ChatCreated(chatDto)).Returns(Task.CompletedTask);

            // Act
            var result = await messagingService.StartChatAsync(otherUser.Id, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(chatDto);
            chatRepositoryMock.Verify(cr => cr.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            messageClientMock.Verify(m => m.ChatCreated(chatDto), Times.Once);
        }

        [Fact]
        public async Task start_chat_should_throw_if_user_was_not_found()
        {
            // Arrange
            var randomId = Guid.NewGuid();

            currentRequestAccessorMock.Setup(cr => cr.Id).Returns(randomId);

            // Act
            var result = async () => await messagingService.StartChatAsync(randomId, CancellationToken.None);

            // Assert
            await result.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"User with Id: {randomId} was not found");
        }

        [Fact]
        public async Task start_chat_should_throw_if_other_user_was_not_found()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid() };
            var randomId = Guid.NewGuid();
                
            currentRequestAccessorMock.Setup(cr => cr.Id).Returns(user.Id);
            userRepositoryMock.Setup(us => us.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);

            // Act
            var result = async () => await messagingService.StartChatAsync(randomId, CancellationToken.None);

            // Assert
            await result.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"User with Id: {randomId} was not found");
        }

        [Fact]
        public async Task send_message_should_create_and_return_message()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid() };
            var chat = new Chat
            {
                Id = Guid.NewGuid(),
                ChatUsers = new List<ChatUser>
                {
                    new ChatUser { UserId = user.Id }
                }
            };
            var message = new Message { Id = Guid.NewGuid(), ChatId = chat.Id, SenderId = user.Id };
            var messageDto = new MessageDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                ChatId = message.ChatId,
            };

            userRepositoryMock.Setup(us => us.GetUserByIdAsync(messageDto.SenderId.Value, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            chatRepositoryMock.Setup(cr => cr.GetChatByIdAsync(messageDto.ChatId.Value, It.IsAny<CancellationToken>())).ReturnsAsync(chat);
            messageRepositoryMock.Setup(mr => mr.AddMessageAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>())).ReturnsAsync(message);
            mapperMock.Setup(m => m.Map<Message>(messageDto)).Returns(message);
            mapperMock.Setup(m => m.Map<MessageDto>(message)).Returns(messageDto);
            messageHubMock.Setup(m => m.Clients.Users(It.IsAny<List<string>>())).Returns(messageClientMock.Object);
            messageClientMock.Setup(m => m.MessageSent(messageDto)).Returns(Task.CompletedTask);

            // Act
            var result = await messagingService.SendMessageAsync(messageDto, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(messageDto);
            messageRepositoryMock.Verify(mr => mr.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            messageClientMock.Verify(m => m.MessageSent(messageDto), Times.Once);
        }

        [Fact]
        public async Task send_message_should_throw_if_chat_does_not_exist()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid() };
            var message = new Message { Id = Guid.NewGuid(), ChatId = Guid.NewGuid(), SenderId = user.Id };
            var messageDto = new MessageDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                ChatId = message.ChatId,
            };

            userRepositoryMock.Setup(us => us.GetUserByIdAsync(messageDto.SenderId.Value, It.IsAny<CancellationToken>())).ReturnsAsync(user);

            // Act
            var result = async () => await messagingService.SendMessageAsync(messageDto, CancellationToken.None);

            // Assert
            await result.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"Chat with Id: {messageDto.ChatId} was not found");
        }

        [Fact]
        public async Task send_message_should_throw_if_chat_does_not_container_sender_id()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid() };
            var chat = new Chat { Id = Guid.NewGuid() };
            var message = new Message { Id = Guid.NewGuid(), ChatId = chat.Id, SenderId = user.Id };
            var messageDto = new MessageDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                ChatId = message.ChatId,
            };

            userRepositoryMock.Setup(us => us.GetUserByIdAsync(messageDto.SenderId.Value, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            chatRepositoryMock.Setup(cr => cr.GetChatByIdAsync(messageDto.ChatId.Value, It.IsAny<CancellationToken>())).ReturnsAsync(chat);

            // Act
            var result = async () => await messagingService.SendMessageAsync(messageDto, CancellationToken.None);

            // Assert
            await result.Should().ThrowAsync<AuthException>().WithMessage("Sender is not a member of the chat.");
        }
    }
}
