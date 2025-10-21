using Microsoft.Playwright;
using PostsByMarko.FrontendTests.Helpers;

namespace PostsTesting.UI_Models.Components
{
    public class Modal : Component
    {
        public Modal(IPage page) : base(page) { }

        public ILocator modalContainer => page.Locator(".modal");
        public ILocator title => page.Locator(".modal__title");
        public ILocator titleInput => page.Locator("#title");
        public ILocator contentInput => page.Locator("#content");
        public ILocator messageFailure => page.Locator(".modal__message.fail");
        public ILocator messageSuccess => page.Locator(".modal__message.success");
        public ILocator createButton => button.GetByText("Create");
        public ILocator updateButton => button.GetByText("Update");
        public ILocator deleteButton => button.GetByText("Delete");
        public ILocator cancelButton => button.GetByText("Cancel");
        

        public async Task FillInTitleInput(string titleToBeEntered)
        {
            await titleInput.FillAsync(titleToBeEntered);
        }

        public async Task FillInContentInput(string contentToBeEntered)
        {
            await contentInput.FillAsync(contentToBeEntered);
        }

        public async Task WaitForSuccessMessageToShowAndDisappear()
        {
            await PlaywrightHelpers.WaitForElementToVisible(successMessage);
            await PlaywrightHelpers.WaitForElementToBeHidden(successMessage);
        }
    }
}
