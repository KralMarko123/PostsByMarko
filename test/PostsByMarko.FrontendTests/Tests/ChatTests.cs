using FluentAssertions;
using Microsoft.Playwright;
using PostsByMarko.FrontendTests.UI_Models.Pages;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Test.Shared.Constants;
using PostsTesting.UI_Models.Pages;
using Xunit;
using Message = PostsByMarko.FrontendTests.UI_Models.Components.Message;

namespace PostsByMarko.FrontendTests.Tests
{
    [Collection("Frontend Collection")]
    public class ChatTests : IAsyncLifetime
    {
        private readonly PostsByMarkoFactory postsByMarkoFactory;
        private IPage page;
        private ChatPage chatPage;
        private LoginPage loginPage;
        private HomePage homePage;
        private readonly User testAdmin = TestingConstants.TEST_ADMIN;
        private readonly User testUser = TestingConstants.TEST_USER;
        private readonly User marko = TestingConstants.MARKO;

        public ChatTests(PostsByMarkoFactory postsByMarkoFactory)
        {
            this.postsByMarkoFactory = postsByMarkoFactory;
        }

        // Setup
        public async Task InitializeAsync()
        {
            page = await postsByMarkoFactory.browser.NewPageAsync();

            homePage = new HomePage(page);
            loginPage = new LoginPage(page);
            chatPage = new ChatPage(page);
        }

        // Teardown
        public async Task DisposeAsync()
        {
            if (page != null) await page.CloseAsync();
        }

        [Fact]
        public async Task should_view_chats()
        {
            await LoginWithUser(testUser, loginPage, homePage);
            await NavigateToChatPage(homePage, chatPage);   

            var usernames = await chatPage.GetUsernames();
            var infoMessageText = await chatPage.infoMessage.TextContentAsync();

            usernames.Should().Contain([$"{marko.FirstName} {marko.LastName}", $"{testAdmin.FirstName} {testAdmin.LastName}"]);
            infoMessageText.Should().Be("Start chatting right away by clicking on another user");
        }

        [Fact]
        public async Task should_send_a_message()
        {
            await LoginWithUser(testAdmin, loginPage, homePage);
            await NavigateToChatPage(homePage, chatPage);

            var userCard = chatPage.GetUserCard($"{marko.FirstName} {marko.LastName}");

            await userCard.ClickAsync();
            await chatPage.messageInput.FillAsync("Hello from admin!");
            await chatPage.sendButton.ClickAsync();

            var lastMessageSent = new Message(page, chatPage.message.Last);
            var messageText = await lastMessageSent.content.TextContentAsync();

            await Assertions.Expect(lastMessageSent.message).ToContainClassAsync("author");
            messageText.Should().Be("Hello from admin!");
        }

        [Fact]
        public async Task shoild_not_send_empty_message()
        {
            await LoginWithUser(testAdmin, loginPage, homePage);
            await NavigateToChatPage(homePage, chatPage);

            var testUsername = $"{testUser.FirstName} {testUser.LastName}";
            var userCard = chatPage.userCard.GetByText(testUsername);

            await userCard.ClickAsync();
            await chatPage.messageList.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

            var sentMessages = await chatPage.message.CountAsync();

            await chatPage.sendButton.ClickAsync();

            var sentMessagesAfterClick = await chatPage.message.CountAsync();

            await Assertions.Expect(chatPage.messageInput).ToContainClassAsync("empty");
            sentMessagesAfterClick.Should().Be(sentMessages);
        }

        [Fact]
        public async Task should_receive_a_message()
        {
            await LoginWithUser(testAdmin, loginPage, homePage);
            await NavigateToChatPage(homePage, chatPage);

            var newPage = await postsByMarkoFactory.browser.NewPageAsync();
            var secondLoginPage = new LoginPage(newPage);
            var secondHomePage = new HomePage(newPage);
            var secondChatPage = new ChatPage(newPage);

            await LoginWithUser(testUser, secondLoginPage, secondHomePage);
            await NavigateToChatPage(secondHomePage, secondChatPage);

            var adminUserCard = secondChatPage.GetUserCard($"{testAdmin.FirstName} {testAdmin.LastName}");
            
            await adminUserCard.ClickAsync();
            await secondChatPage.messageInput.FillAsync("Hello from test user!");
            await secondChatPage.sendButton.ClickAsync();

            await chatPage.WaitForNotificationToRegister();
            await chatPage.BringToFront();

            var testUserCard = chatPage.GetUserCard($"{testUser.FirstName} {testUser.LastName}");
            var unreadMessages = await testUserCard.Locator(".user-unreads").TextContentAsync();

            await testUserCard.ClickAsync();
            await chatPage.messageList.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

            var lastMessageReceived = new Message(page, chatPage.message.Last);
            var messageText = await lastMessageReceived.content.TextContentAsync();

            unreadMessages.Should().NotBeNullOrEmpty();
            messageText.Should().Be("Hello from test user!");
        }


        private async Task LoginWithUser(User user, LoginPage loginPage, HomePage homePage)
        {
            await loginPage.Visit();
            await loginPage.Login(user.Email, TestingConstants.TEST_PASSWORD);
            await homePage.username.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        }

        private async Task NavigateToChatPage(HomePage homePage, ChatPage chatPage)
        {
            await homePage.navComponent.dropdownMenu.HoverAsync();
            await homePage.navComponent.chat.ClickAsync();
            await chatPage.chatContainer.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        }
    }
}
