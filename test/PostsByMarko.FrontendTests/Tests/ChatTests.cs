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
            await LoginWithUser(testUser);

            await homePage.navComponent.dropdownMenu.HoverAsync();
            await homePage.navComponent.chat.ClickAsync();
            await chatPage.chatContainer.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

            var usernames = await chatPage.GetUsernames();
            var infoMessageText = await chatPage.infoMessage.TextContentAsync();

            infoMessageText.Should().Be("Start chatting right away by clicking on another user");
            usernames.Should().NotBeEmpty();
            usernames.Should().Contain([$"{marko.FirstName} {marko.LastName}", $"{testAdmin.FirstName} {testAdmin.LastName}"]);
        }

        [Fact]
        public async Task should_send_a_message()
        {
            await LoginWithUser(testAdmin);

            await homePage.navComponent.dropdownMenu.HoverAsync();
            await homePage.navComponent.chat.ClickAsync();
            await chatPage.chatContainer.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

            var testUsername = $"{testUser.FirstName} {testUser.LastName}";
            var userCard = chatPage.userCard.GetByText(testUsername);

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
            await LoginWithUser(testAdmin);

            await homePage.navComponent.dropdownMenu.HoverAsync();
            await homePage.navComponent.chat.ClickAsync();
            await chatPage.chatContainer.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

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


        private async Task LoginWithUser(User user)
        {
            await loginPage.Visit();
            await loginPage.Login(user.Email, TestingConstants.TEST_PASSWORD);
            await homePage.username.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        }
    }
}
