using Microsoft.Playwright;
using PostsTesting.Utility.Constants;
using Xunit;

namespace PostsTesting.Utility.Pages
{
    public class PostDetailsPage
    {
        private IPage page;
        private static string url => $"{AppConstants.UiEndpoint}/posts";
        public PostDetailsPage(IPage page) => this.page = page;


        public ILocator title => page.Locator(".container__title");
        public ILocator content => page.Locator(".container__description");
        public ILocator backButton => page.Locator(".container__back");


        public async Task Visit(string postId)
        {
            await page.GotoAsync($"{url}/{postId}");
        }

        public async Task CheckPostDetails(string expectedTitle, string expectedContent)
        {
            await WaitForTitleToAppear();

            bool detailsElementsAreDisplayed = await title.IsVisibleAsync() &&
                await content.IsVisibleAsync() &&
                await backButton.IsVisibleAsync();
            string titleText = await title.TextContentAsync();
            string contentText = await content.TextContentAsync();

            Assert.True(detailsElementsAreDisplayed);
            Assert.Equal(expectedTitle, titleText);
            Assert.Equal(expectedContent, contentText);
        }

        public async Task GoBack()
        {
            await backButton.ClickAsync();
        }

        public async Task WaitForTitleToAppear()
        {
            await title.WaitForAsync();
        }
    }
}
