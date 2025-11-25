//using Azure;
//using FluentAssertions;
//using Newtonsoft.Json;
//using PostsByMarko.Host.Data.Entities;
//using PostsByMarko.Test.Shared.Constants;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Json;
//using System.Threading.Tasks;
//using Xunit;

//namespace PostsByMarko.IntegrationTests
//{
//    [Collection("Integration Collection")]
//    public class MessageControllerTests
//    {
//        private readonly HttpClient client;
//        private readonly User testAdmin = TestingConstants.TEST_ADMIN;
//        private readonly User testUser = TestingConstants.TEST_USER;
//        private readonly User randomUser = TestingConstants.RANDOM_USER;

//        public MessageControllerTests(PostsByMarkoApiFactory postsByMarkoApiFactory)
//        {
//            client = postsByMarkoApiFactory.client!;
//        }

//        [Fact]
//        public async Task should_create_chat()
//        {
//            // Arrange
//            var participantIds = new string[] { testAdmin.Id, testUser.Id };

//            // Act
//            var response = await client.PostAsJsonAsync("/startChat", participantIds);
//            var requestResult = await response.Content.ReadFromJsonAsync<RequestResult>();
//            var chat = JsonConvert.DeserializeObject<Chat>(requestResult.Payload.ToString());

//            // Assert
//            requestResult.Should().NotBeNull();
//            requestResult.StatusCode.Should().Be(HttpStatusCode.OK);

//            chat.Should().NotBeNull();
//            chat.ParticipantIds.Should().BeEquivalentTo(participantIds);
//        }

//        [Fact]
//        public async Task should_send_a_message()
//        {
//            // Arrange
//            var participantIds = new string[] { testAdmin.Id, testUser.Id };

//            var response = await client.PostAsJsonAsync("/startChat", participantIds);
//            var requestResult = await response.Content.ReadFromJsonAsync<RequestResult>();
//            var chat = JsonConvert.DeserializeObject<Chat>(requestResult.Payload.ToString());
//            var messageToSend = new MessageDto(chat.Id, testAdmin.Id, "Test");

//            // Act
//            response = await client.PostAsJsonAsync("/sendMessage", messageToSend);
//            requestResult = await response.Content.ReadFromJsonAsync<RequestResult>();
//            var message = JsonConvert.DeserializeObject<Message>(requestResult.Payload.ToString());

//            // Assert
//            requestResult.Should().NotBeNull();
//            requestResult.StatusCode.Should().Be(HttpStatusCode.OK);

//            message.Should().NotBeNull();
//            message.SenderId.Should().Be(testAdmin.Id);
//            message.Content.Should().Be(messageToSend.Content);
//            message.ChatId.Should().Be(messageToSend.ChatId);
//        }

//        [Fact]
//        public async Task should_get_chats_for_user()
//        {
//            // Arrange
//            var firstChatParticipants = new string[] { testAdmin.Id, testUser.Id };
//            var secondChatParticipants = new string[] { testAdmin.Id, randomUser.Id };

//            var firstChat = await GetChatForParticipants(firstChatParticipants);
//            var secondChat = await GetChatForParticipants(secondChatParticipants);

//            await client.PostAsJsonAsync("/sendMessage", new MessageDto(firstChat.Id, testAdmin.Id, "First Message"));
//            await client.PostAsJsonAsync("/sendMessage", new MessageDto(secondChat.Id, testAdmin.Id, "Second Message"));

//            // Act
//            var requestResult = await client.GetFromJsonAsync<RequestResult>("/getChats");
//            var chats = JsonConvert.DeserializeObject<List<Chat>>(requestResult.Payload.ToString());

//            // Assert
//            requestResult.Should().NotBeNull();
//            requestResult.StatusCode.Should().Be(HttpStatusCode.OK);

//            chats.Should().NotBeNull();
//            chats.Count.Should().Be(2);

//            chats[0].ParticipantIds.Should().BeEquivalentTo(firstChatParticipants);
//            chats[0].Id.Should().Be(firstChat.Id);
//            chats[0].Messages.First().Content.Should().Be("First Message");


//            chats[1].ParticipantIds.Should().BeEquivalentTo(secondChatParticipants);
//            chats[1].Id.Should().Be(secondChat.Id);
//            chats[1].Messages.First().Content.Should().Be("Second Message");
//        }

//        private async Task<Chat> GetChatForParticipants(string[] participants)
//        {
//            var response = await client.PostAsJsonAsync("/startChat", participants);
//            var requestResult = await response.Content.ReadFromJsonAsync<RequestResult>();
//            var chat = JsonConvert.DeserializeObject<Chat>(requestResult.Payload.ToString());

//            return chat;
//        }
//    }
//}
