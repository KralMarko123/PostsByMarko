using Microsoft.Playwright;
using PostsTesting.UI_Models.Pages;

namespace PostsByMarko.FrontendTests.UI_Models.Pages
{
    public class ChatPage : Page
    {
        private static string url => $"{baseUrl}/chat";

        public ChatPage(IPage page) : base(page) { }

        public ILocator chatContainer => page.Locator(".chat-container");
        public ILocator userList => page.Locator(".user-list");
        public ILocator messageList => page.Locator(".message-list");
        public ILocator messageInput => page.Locator(".message-input");
        public ILocator sendButton => page.Locator(".send-icon");
        public ILocator infoMessage => page.Locator(".info-message");
        public ILocator userCard => page.Locator(".user-card");
        public ILocator username => page.Locator(".user-name");
        public ILocator message => page.Locator(".message");

        public async Task<List<string>> GetUsernames()
        {
            var usernames = await username.AllTextContentsAsync();

            return [.. usernames];
        }


        public async Task Visit()
        {
            await page.GotoAsync(url);
        }
    }
}
