using FluentAssertions;
using Moq;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Host.Data.Models.Dtos;
using PostsByMarko.Host.Repos.Messaging;
using PostsByMarko.Host.Services;
using System.Net;

namespace PostsByMarko.UnitTests
{
    public class MessagingServiceTests
    {
        private readonly MessagingService service;
        private readonly Mock<IMessagingRepository> messagingRepositoryMock = new Mock<IMessagingRepository>();

        public MessagingServiceTests()
        {
            service = new MessagingService(messagingRepositoryMock.Object);
        }

        [Fact]
        public async Task start_chat_should_return_a_new_chat_for_participants_if_no_previous_chat_existed()
        {
            // Arrange
            var firstParticipantId = Guid.NewGuid().ToString();
            var secondParticipantId = Guid.NewGuid().ToString();
            var participantIds = new string[] { firstParticipantId, secondParticipantId };

            messagingRepositoryMock.Setup(r => r.GetChatByParticipantIdsAsync(participantIds)).ReturnsAsync(() => null);
            messagingRepositoryMock.Setup(r => r.CreateChatAsync(It.IsAny<Chat>())).ReturnsAsync(() => new Chat([ .. participantIds]));

            // Act
            var result = await service.StartChatAsync(participantIds);
            var newChat = result.Payload as Chat;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.OK);

            newChat.Should().NotBeNull();
            newChat.ParticipantIds.Should().Equal(participantIds);
            newChat.Messages.Should().BeEmpty();
        }

        [Fact]
        public async Task start_chat_should_return_an_existing_chat_for_participants()
        {
            // Arrange
            var firstParticipantId = Guid.NewGuid().ToString();
            var secondParticipantId = Guid.NewGuid().ToString();
            var participantIds = new string[] { firstParticipantId, secondParticipantId };
            var existingChat = new Chat([.. participantIds]);
            var existingMessages = new List<Message>()
            {
                new Message(existingChat.Id, firstParticipantId, "First"),
                new Message(existingChat.Id, firstParticipantId, "First")
            };

            messagingRepositoryMock.Setup(r => r.GetChatByParticipantIdsAsync(participantIds)).ReturnsAsync(() => existingChat);
            messagingRepositoryMock.Setup(r => r.GetChatMessagesAsync(existingChat)).ReturnsAsync(() => existingMessages);

            // Act
            var result = await service.StartChatAsync(participantIds);
            var chat = result.Payload as Chat;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.OK);

            chat.Should().NotBeNull();
            chat.ParticipantIds.Should().Equal(participantIds);
            chat.Messages.Should().BeEquivalentTo(existingMessages);
        }

        [Fact]
        public async Task send_message_should_return_message_if_successfully_sent()
        {
            // Arrange
            var firstParticipantId = Guid.NewGuid().ToString();
            var secondParticipantId = Guid.NewGuid().ToString();
            var participantIds = new string[] { firstParticipantId, secondParticipantId };
            var existingChat = new Chat([.. participantIds]);
            var existingChatUpdatedAt = existingChat.UpdatedAt;
            var messageDto = new MessageDto(existingChat.Id, firstParticipantId, "Test");
            var expectedMessage = new Message(messageDto);
            

            messagingRepositoryMock.Setup(r => r.GetChatByIdAsync(messageDto.ChatId)).ReturnsAsync(() => existingChat);
            messagingRepositoryMock.Setup(r => r.CreateMessageAsync(It.IsAny<Message>())).ReturnsAsync(() => expectedMessage);
            messagingRepositoryMock.Setup(r => r.UpdateChatAsync(existingChat)).ReturnsAsync(() => true);

            // Act
            var result = await service.SendMessageAsync(messageDto);
            var message = result.Payload as Message;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.OK);

            message.Should().NotBeNull();
            message.ChatId.Should().Be(existingChat.Id);
            message.SenderId.Should().Be(firstParticipantId);
            message.Content.Should().Be("Test");

            existingChat.Messages.Should().NotBeNull();
            existingChat.Messages.Should().Contain(message);
            existingChat.UpdatedAt.Should().BeAfter(existingChatUpdatedAt);
        }

        [Fact]
        public async Task send_message_should_return_not_found_if_chat_does_not_exist()
        {
            // Arrange
            var firstParticipantId = Guid.NewGuid().ToString();
            var secondParticipantId = Guid.NewGuid().ToString();
            var participantIds = new string[] { firstParticipantId, secondParticipantId };
            var messageDto = new MessageDto(1, firstParticipantId, "Test");

            messagingRepositoryMock.Setup(r => r.GetChatByIdAsync(messageDto.ChatId)).ReturnsAsync(() => null);
            
            // Act
            var result = await service.SendMessageAsync(messageDto);

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Message.Should().Be($"Chat with Id: {messageDto.ChatId} was not found");
            result.Payload.Should().BeNull();
        }

        [Fact]
        public async Task send_message_should_return_forbidden_if_sender_is_not_a_participant()
        {
            // Arrange
            var forbiddenParticipantId = Guid.NewGuid().ToString();
            var firstParticipantId = Guid.NewGuid().ToString();
            var secondParticipantId = Guid.NewGuid().ToString();
            var participantIds = new string[] { firstParticipantId, secondParticipantId };
            var existingChat = new Chat([.. participantIds]);
            var messageDto = new MessageDto(existingChat.Id, forbiddenParticipantId, "Test");

            messagingRepositoryMock.Setup(r => r.GetChatByIdAsync(messageDto.ChatId)).ReturnsAsync(() => existingChat);

            // Act
            var result = await service.SendMessageAsync(messageDto);

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            result.Message.Should().Be("You are not a participant of this chat");
            result.Payload.Should().BeNull();
        }

        [Fact]
        public async Task send_message_should_return_bad_request_if_chat_does_not_update()
        {
            // Arrange
            var firstParticipantId = Guid.NewGuid().ToString();
            var secondParticipantId = Guid.NewGuid().ToString();
            var participantIds = new string[] { firstParticipantId, secondParticipantId };
            var existingChat = new Chat([.. participantIds]);
            var messageDto = new MessageDto(existingChat.Id, firstParticipantId, "Test");
            var expectedMessage = new Message(messageDto);

            messagingRepositoryMock.Setup(r => r.GetChatByIdAsync(messageDto.ChatId)).ReturnsAsync(() => existingChat);
            messagingRepositoryMock.Setup(r => r.CreateMessageAsync(It.IsAny<Message>())).ReturnsAsync(() => expectedMessage);
            messagingRepositoryMock.Setup(r => r.UpdateChatAsync(existingChat)).ReturnsAsync(() => false);

            // Act
            var result = await service.SendMessageAsync(messageDto);

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Message.Should().Be("Error during chat update");
            result.Payload.Should().BeNull();
        }
    }
}
