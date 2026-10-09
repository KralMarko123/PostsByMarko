using Microsoft.Playwright;
using PostsByMarko.FrontendTests.Helpers;

namespace PostsTesting.UI_Models.Components
{
    public class Modal : Component
    {
        public Modal(IPage page) : base(page) { }

        public ILocator modalContainer => page.GetByRole(AriaRole.Dialog);
        public ILocator title => modalContainer.GetByRole(AriaRole.Heading);
        public ILocator titleInput => modalContainer.GetByLabel("Title", new LocatorGetByLabelOptions { Exact = true });
        public ILocator contentInput => modalContainer.GetByLabel("Content", new LocatorGetByLabelOptions { Exact = true });
        public ILocator messageFailure => modalContainer.GetByRole(AriaRole.Alert);
        public ILocator messageSuccess => modalContainer.GetByRole(AriaRole.Status);
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
