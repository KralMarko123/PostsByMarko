using FluentAssertions;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Test.Shared.Constants;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace PostsByMarko.IntegrationTests.Hubs
{
    [Collection("IntegrationCollection")]
    public class MessageHubTests : IAsyncLifetime
    {
        private readonly PostsByMarkoApiFactory postsByMarkoApiFactory;
        private readonly HttpClient client;
        private HubConnection? hubConnection;
        private readonly string controllerPrefix = "/api/messaging";

        public MessageHubTests(PostsByMarkoApiFactory postsByMarkoApiFactory)
        {
            this.postsByMarkoApiFactory = postsByMarkoApiFactory;

            client = postsByMarkoApiFactory.client!;
        }

        public async Task InitializeAsync()
        {
            await postsByMarkoApiFactory.RecreateAndSeedDatabaseAsync();
            await postsByMarkoApiFactory.AuthenticateClientAsync(client);

            hubConnection = postsByMarkoApiFactory.CreateHubConnection("messageHub");

            await hubConnection.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await hubConnection!.StopAsync();
            await hubConnection.DisposeAsync();
        }

        [Fact]
        public async Task creating_a_chat_notifies_users()
        {
            // Arrange
            var testAdmin = await postsByMarkoApiFactory.GetUserByEmailAsync(TestingConstants.TEST_ADMIN_EMAIL);
            var testUser = await postsByMarkoApiFactory.GetUserByEmailAsync(TestingConstants.TEST_USER_EMAIL);

            ChatDto? chatCreated = null;
            hubConnection!.On<ChatDto>("ChatCreated", chat => chatCreated = chat);

            // Act
            await client.PostAsync($"{controllerPrefix}/chats/user/{testUser.Id}", null);
            await postsByMarkoApiFactory.WaitForSignalRPropagation();

            // Assert
            chatCreated.Should().NotBeNull();
            chatCreated.Users.Should().HaveCount(2);
            chatCreated.Users.Select(u => u.Id).Should().Contain(id => id == testAdmin.Id);
            chatCreated.Users.Select(u => u.Id).Should().Contain(id => id == testUser.Id);
        }

        [Fact]
        public async Task sending_a_message_notifies_users()
        {
            // Arrange
            var testAdmin = await postsByMarkoApiFactory.GetUserByEmailAsync(TestingConstants.TEST_ADMIN_EMAIL);
            var testUser = await postsByMarkoApiFactory.GetUserByEmailAsync(TestingConstants.TEST_USER_EMAIL);
            var chat = await StartChatAsync(testUser.Id);
            var messageDto = new MessageDto
            {
                ChatId = chat.Id,
                Content = "This is a message sent during SignalR tests.",
                SenderId = testAdmin.Id
            };

            MessageDto? messageSent = null;
            hubConnection!.On<MessageDto>("MessageSent", message => messageSent = message);

            // Act
            await client.PostAsJsonAsync($"{controllerPrefix}/send", messageDto);
            await postsByMarkoApiFactory.WaitForSignalRPropagation();

            // Assert
            messageSent.Should().NotBeNull();
            messageSent.ChatId.Should().Be(chat.Id);
            messageSent.SenderId.Should().Be(testAdmin.Id);
            messageSent.Content.Should().Be(messageDto.Content);
        }

        private async Task<ChatDto> StartChatAsync(Guid otherUserId)
        {
            var response = await client.PostAsync($"{controllerPrefix}/chats/user/{otherUserId}", null);
            var responseContent = await response.Content.ReadAsStringAsync();
            var chat = JsonConvert.DeserializeObject<ChatDto>(responseContent);

            return chat!;
        }
    }
}
