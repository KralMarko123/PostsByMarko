using Microsoft.Playwright;
using Xunit;

namespace PostsTesting.Utility.Pages
{
    public class Modal
    {

        private IPage page;
        public ILocator modal => page.Locator(".modal");
        public ILocator title => page.Locator(".modal__title");
        public ILocator cancelButton => page.Locator(".form__actions .button", new PageLocatorOptions { HasTextString = "Cancel" });

        public Modal(IPage page) => this.page = page;

        public async Task CheckVisibility(string expectedTitleText)
        {
            bool modalIsDisplayed = await modal.IsVisibleAsync() && await title.IsVisibleAsync();
            string titleText = await title.TextContentAsync();
            Assert.True(modalIsDisplayed);
            Assert.Equal(expectedTitleText, titleText);
        }

        public async Task CloseModal()
        {
            await cancelButton.ClickAsync();
            await modal.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Detached });
        }
    }
}
