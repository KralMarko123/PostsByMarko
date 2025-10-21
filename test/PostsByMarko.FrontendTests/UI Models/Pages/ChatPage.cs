using Microsoft.Playwright;
using PostsByMarko.FrontendTests.Helpers;
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
        public ILocator message => page.Locator(".message");
        public ILocator unreads => page.Locator(".user-unreads");
        public ILocator username => page.Locator(".user-name");

        public async Task<List<string>> GetUsernames()
        {
            var usernames = new List<string>();
            var userCards = await userCard.AllAsync();

            foreach (var card in userCards)
            { 
                var name = await card.Locator(".user-name").TextContentAsync();
                if (name != null)
                {
                    usernames.Add(name);
                }
            }

            return [.. usernames];
        }

        public ILocator GetUserCard(string withUsername)
        {
            return userCard.Filter(new LocatorFilterOptions { HasText = withUsername });
        }

        public async Task WaitForNotificationToRegister()
        {
            await PlaywrightHelpers.WaitForCountToChange(unreads);
        }

        public async Task Visit()
        {
            await page.GotoAsync(url);
        }
    }
}
