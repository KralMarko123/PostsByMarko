using FluentAssertions;
using Microsoft.Playwright;
using PostsByMarko.FrontendTests.UI_Models.Pages;
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
        private readonly string testAdminEmail = TestingConstants.TEST_ADMIN_EMAIL;
        private readonly string testUserEmail = TestingConstants.TEST_USER_EMAIL;
        private readonly string testAdminFullname = "Test Admin";
        private readonly string testUserFullname = "Test User";
        private readonly string ownerFullname = "Marko Markovikj";
        private readonly string randomUserFullname = "User Userson";

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
            await LoginWithEmail(testUserEmail, loginPage, homePage);
            await NavigateToChatPage(homePage, chatPage);

            var infoMessageText = await chatPage.infoMessage.TextContentAsync();
            var userListShown = await chatPage.userList.IsVisibleAsync();

            userListShown.Should().BeTrue();
            infoMessageText.Should().Be("Start chatting right away by clicking on another user");
        }

        [Fact]
        public async Task should_send_a_message()
        {
            await LoginWithEmail(testAdminEmail, loginPage, homePage);
            await NavigateToChatPage(homePage, chatPage);

            var userCard = chatPage.GetUserCard(randomUserFullname);

            await userCard.ClickAsync();
            await chatPage.messageList.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
            await chatPage.messageInput.FillAsync("Hello from admin!");
            await chatPage.sendButton.ClickAsync();
            await chatPage.WaitForNumberOfMessagesToChange();

            var lastMessageSent = new Message(page, chatPage.message.Last);
            var messageText = await lastMessageSent.content.TextContentAsync();

            await Assertions.Expect(lastMessageSent.message).ToContainClassAsync("author");
            messageText.Should().Be("Hello from admin!");
        }

        [Fact]
        public async Task shoild_not_send_empty_message()
        {
            await LoginWithEmail(testAdminEmail, loginPage, homePage);
            await NavigateToChatPage(homePage, chatPage);

            var testUsername = testUserFullname;
            var userCard = chatPage.userCard.GetByText(testUsername).First;

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
            await LoginWithEmail(testAdminEmail, loginPage, homePage);
            await NavigateToChatPage(homePage, chatPage);

            var newPage = await postsByMarkoFactory.browser.NewPageAsync();
            var secondLoginPage = new LoginPage(newPage);
            var secondHomePage = new HomePage(newPage);
            var secondChatPage = new ChatPage(newPage);

            await LoginWithEmail(testUserEmail, secondLoginPage, secondHomePage);
            await NavigateToChatPage(secondHomePage, secondChatPage);

            var adminUserCard = secondChatPage.GetUserCard(testAdminFullname);

            await adminUserCard.ClickAsync();
            await secondChatPage.messageInput.FillAsync("Hello from test user!");
            await secondChatPage.sendButton.ClickAsync();

            await chatPage.WaitForNotificationToRegister();
            await chatPage.BringToFront();

            var testUserCard = chatPage.GetUserCard(testUserFullname);
            var unreadMessages = await testUserCard.Locator(".user-unreads").TextContentAsync();

            await testUserCard.ClickAsync();
            await chatPage.messageList.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

            var lastMessageReceived = new Message(page, chatPage.message.Last);
            var messageText = await lastMessageReceived.content.TextContentAsync();

            unreadMessages.Should().NotBeNullOrEmpty();
            messageText.Should().Be("Hello from test user!");
        }


        private static async Task LoginWithEmail(string email, LoginPage loginPage, HomePage homePage)
        {
            await loginPage.Visit();
            await loginPage.Login(email, TestingConstants.TEST_PASSWORD);
            await homePage.username.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        }

        private static async Task NavigateToChatPage(HomePage homePage, ChatPage chatPage)
        {
            await homePage.navComponent.dropdownMenu.HoverAsync();
            await homePage.navComponent.chat.ClickAsync();
            await chatPage.chatContainer.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
            await chatPage.userList.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        }
    }
}
