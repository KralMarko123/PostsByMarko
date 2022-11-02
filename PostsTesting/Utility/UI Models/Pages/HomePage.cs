using Microsoft.Playwright;
using PostsTesting.Utility.Constants;
using Xunit;

namespace PostsTesting.Utility.Pages
{
    public class HomePage
    {
        private static string url => $"{AppConstants.UiEndpoint}/";

        private IPage page;
        public ILocator title => page.Locator(".container__title");
        public ILocator subtitles => page.Locator(".container__description");
        public ILocator createPostButton => page.Locator(".button");

        public Modal modal => new Modal(page);

        public HomePage(IPage page) => this.page = page;

        public async Task Visit()
        {
            await page.GotoAsync(url);
        }

        public async Task CheckDefaultState()
        {
            bool homeElementsAreDisplayed = await title.IsVisibleAsync() && await subtitles.IsVisibleAsync() && await createPostButton.IsVisibleAsync();
            string titleText = await title.TextContentAsync();
            Assert.True(homeElementsAreDisplayed);
            Assert.Equal("Welcome to our blog!", titleText);

            await createPostButton.ClickAsync();
            await modal.CheckVisibility("Create Form");
        }
    }
}
