using Microsoft.Playwright;
using PostsTesting.Utility.Constants;

namespace PostsTesting.Utility.Pages
{
    public class HomePage
    {
        private static string url => $"{AppConstants.UiEndpoint}/";

        private IPage page;
        public ILocator title => page.Locator("#title", new PageLocatorOptions()
        { HasTextString = "Welcome to our blog!" });

        public HomePage(IPage page) => this.page = page;

        public async Task Visit()
        {
            await page.GotoAsync(url);
        }
    }
}
