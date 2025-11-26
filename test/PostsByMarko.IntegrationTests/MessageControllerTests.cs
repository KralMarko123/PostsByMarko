using FluentAssertions;
using Newtonsoft.Json;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Test.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace PostsByMarko.IntegrationTests
{
    [Collection("IntegrationCollection")]
    public class MessageControllerTests : IAsyncLifetime
    {
        private readonly PostsByMarkoApiFactory postsByMarkoApiFactory;
        private readonly HttpClient client;
        private readonly string controllerPrefix = "/api/messaging";

        public MessageControllerTests(PostsByMarkoApiFactory postsByMarkoApiFactory)
        {
            this.postsByMarkoApiFactory = postsByMarkoApiFactory;

            client = postsByMarkoApiFactory.client!;
        }

        public async Task InitializeAsync()
        {
            await postsByMarkoApiFactory.RecreateAndSeedDatabaseAsync();
            await postsByMarkoApiFactory.AuthenticateClientAsync(client);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task should_get_chats()
        {
            // Arrange

            // Act
            var response = await client.GetAsync($"{controllerPrefix}/chats");
            var responseContent = await response.Content.ReadAsStringAsync();
            var chats = JsonConvert.DeserializeObject<List<ChatDto>>(responseContent);

            // Assert
            chats.Should().NotBeNull();
            chats.Should().BeOfType<List<ChatDto>>();
        }

        [Fact]
        public async Task should_start_a_chat()
        {
            // Arrange
            var testAdmin = await postsByMarkoApiFactory.GetUserByEmailAsync(TestingConstants.TEST_ADMIN_EMAIL);
            var testUser = await postsByMarkoApiFactory.GetUserByEmailAsync(TestingConstants.TEST_USER_EMAIL);

            // Act
            var chat = await StartChatAsync(testUser.Id);

            // Assert
            chat.Should().NotBeNull();
            chat.Users.Should().Contain(u => u.Id == testAdmin.Id);
            chat.Users.Should().Contain(u => u.Id == testUser.Id);
            chat.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task should_send_a_message()
        {
            // Arrange
            var testAdmin = await postsByMarkoApiFactory.GetUserByEmailAsync(TestingConstants.TEST_ADMIN_EMAIL);
            var testUser = await postsByMarkoApiFactory.GetUserByEmailAsync(TestingConstants.TEST_USER_EMAIL);
            var chat = await StartChatAsync(testUser.Id);
            var newMessage = new MessageDto
            {
                ChatId = chat.Id,
                SenderId = testAdmin.Id,
                Content = "Hello, this is a test message!"
            };

            // Act
            var response = await client.PostAsJsonAsync($"{controllerPrefix}/send", newMessage);
            var responseContent = await response.Content.ReadAsStringAsync();
            var message = JsonConvert.DeserializeObject<MessageDto>(responseContent);

            // Assert
            message.Should().NotBeNull();
            message.Content.Should().Be(newMessage.Content);
            message.ChatId.Should().Be(chat.Id);
            message.SenderId.Should().Be(testAdmin.Id);
            message.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
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
