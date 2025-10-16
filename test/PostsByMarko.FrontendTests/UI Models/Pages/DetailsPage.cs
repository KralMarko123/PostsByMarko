using Microsoft.Playwright;
using PostsByMarko.FrontendTests.Helpers;
using PostsTesting.UI_Models.Components;

namespace PostsTesting.UI_Models.Pages
{
    public class DetailsPage : Page
    {
        private static string url => $"{baseUrl}/posts";
        public readonly Modal modal;

        public DetailsPage(IPage page) : base(page)
        {
            modal = new Modal(page);
        }

        public ILocator header => page.Locator(".details-header");
        public ILocator title => page.Locator(".details-title");
        public ILocator author => page.Locator(".author");
        public ILocator date => page.Locator(".date");
        public ILocator content => page.Locator(".content");
        public ILocator editButton => button.GetByText("Edit");
        public ILocator backButton => button.GetByText("Back");
        public ILocator saveButton => button.GetByText("Save");
        public ILocator cancelButton => button.GetByText("Cancel");
        public ILocator textArea => page.Locator("textarea[style*='display: block']").Filter(new LocatorFilterOptions { Visible = true });
        public ILocator successMessage => page.Locator(".success.fade-out");

        public async Task Visit(string postId)
        {
            await page.GotoAsync($"{url}/{postId}");
        }

        public async Task WaitForSuccessMessage()
        {
            await PlaywrightHelpers.WaitForElementToVisible(successMessage);
        }

        public async Task WaitForPage()
        {
            await PlaywrightHelpers.WaitForElementToVisible(header);
        }
    }
}
