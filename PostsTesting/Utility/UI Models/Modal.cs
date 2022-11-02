using Microsoft.Playwright;
using PostsTesting.Utility.Constants;
using Xunit;

namespace PostsTesting.Utility.Pages
{
    public class Modal
    {

        private IPage page;
        public ILocator modal => page.Locator(".modal");
        public ILocator title => page.Locator(".modal__title");
        public ILocator buttons => page.Locator(".form__actions .button");

        public Modal(IPage page) => this.page = page;

        public async Task CheckVisibility(string expectedTitleText)
        {
            bool modalIsDisplayed = await modal.IsVisibleAsync() && await title.IsVisibleAsync();
            string titleText = await title.TextContentAsync();
            Assert.True(modalIsDisplayed);
            Assert.Equal(expectedTitleText, titleText);
        }
    }
}
