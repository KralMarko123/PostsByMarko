using Microsoft.Playwright;
using PostsTesting.UI_Models.Components;

namespace PostsTesting.UI_Models.Pages
{
    public class PostDetailsPage : Page
    {
        private static string url => $"{baseUrl}/posts";
        public readonly Modal modal;
        public readonly Select select;

        public PostDetailsPage(IPage page) : base(page)
        {
            modal = new Modal(page);
            select = new Select(page);
        }

        public ILocator toggleUsersButton => page.Locator(".footer__action", new PageLocatorOptions { HasText = "Toggle Users" });

        public async Task Visit(string postId)
        {
            await page.GotoAsync($"{url}/{postId}");
        }

        public async Task ClickOnToggleUsersButton()
        {
            await toggleUsersButton.ClickAsync();
        }
    }
}
