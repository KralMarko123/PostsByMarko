using FluentAssertions;
using Microsoft.Playwright;

namespace PostsTesting.Utility.UI_Models.Pages
{
    public class PostDetailsPage : Page
    {
        private static string url => $"{baseUrl}/posts";
        public PostDetailsPage(IPage page) : base(page) { }


        public async Task Visit(string postId)
        {
            await page.GotoAsync($"{url}/{postId}");
        }

        public async Task ClickOnBackButton()
        {
            await back.ClickAsync();
        }

        public async Task CheckPostDetails(string expectedTitle, string expectedContent)
        {
            await title.WaitForAsync();

            var detailsElementsAreDisplayed = await title.IsVisibleAsync() && await description.IsVisibleAsync();
            var titleText = await title.TextContentAsync();
            var descriptionText = await description.TextContentAsync();

            detailsElementsAreDisplayed.Should().BeTrue();

            titleText.Should().Be(expectedTitle);
            descriptionText.Should().Be(expectedContent);
        }
    }
}
