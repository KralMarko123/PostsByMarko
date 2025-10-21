using Microsoft.Playwright;
using PostsByMarko.Test.Shared.Constants;

namespace PostsTesting.UI_Models.Pages
{
    public class Page : Shared
    {
        protected static string baseUrl => $"{TestingConstants.DEV_CLIENT_ENDPOINT}";

        public Page(IPage page) : base(page) 
        {
            page.SetViewportSizeAsync(1920, 1080);
        }

        public ILocator containerTitle => page.Locator(".container-title");
        public ILocator containerDescription => page.Locator(".container-desc");

        public async Task BringToFront()
        {
            await page.BringToFrontAsync();
        }
    }
}
