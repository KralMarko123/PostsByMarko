using Microsoft.Playwright;
using PostsByMarko.FrontendTests.UI_Models.Components;
using PostsTesting.UI_Models.Components;

namespace PostsTesting.UI_Models.Pages
{
    public class HomePage : Page
    {
        private static string url => $"{baseUrl}/";
        public readonly Modal modalComponent;
        public readonly Nav navComponent;

        public HomePage(IPage page) : base(page)
        {
            modalComponent = new Modal(page);
            navComponent = new Nav(page);
        }

        public ILocator home => page.Locator(".home");
        public ILocator username => page.Locator(".nav__username");
        public ILocator postCard => page.Locator(".post");
        public ILocator postList => page.Locator(".posts-list");
       
        public async Task Visit()
        {
            await page.GotoAsync(url);
        }
    }
}
