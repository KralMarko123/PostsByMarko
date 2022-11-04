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

        public async Task GoBack()
        {
            await backButton.ClickAsync();
        }

        public async Task CheckPostDetails(string expectedTitle, string expectedContent)
        {
            await WaitForTitleToAppear();

            var detailsElementsAreDisplayed = await title.IsVisibleAsync() &&
                await content.IsVisibleAsync() &&
                await backButton.IsVisibleAsync();
            var titleText = await title.TextContentAsync();
            var contentText = await content.TextContentAsync();

            Assert.True(detailsElementsAreDisplayed);
            Assert.Equal(expectedTitle, titleText);
            Assert.Equal(expectedContent, contentText);
        }

        public async Task WaitForTitleToAppear()
        {
            await title.WaitForAsync();
        }

        public async Task<IResponse> GetResponseForPostDetails(string postId)
        {
            IResponse postDetailsResponse = await page.RunAndWaitForResponseAsync(async () => await Visit(postId),
            r => r.Url.Contains($"get-post-by-id/{postId}"));
            return postDetailsResponse;
        }
    }
}
