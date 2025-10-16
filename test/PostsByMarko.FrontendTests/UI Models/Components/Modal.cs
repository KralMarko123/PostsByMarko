using FluentAssertions;
using Microsoft.Playwright;

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


        public async Task CloseModal()
        {
            await WaitForModalToBeRemoved();
        }

        public async Task FillInTitleInput(string titleToBeEntered)
        {
            await titleInput.FillAsync(titleToBeEntered);
        }

        public async Task FillInContentInput(string contentToBeEntered)
        {
            await contentInput.FillAsync(contentToBeEntered);
        }

        public async Task FillInFormAndSubmit(string titleToBeEntered, string contentToBeEntered, string expectedMessage = null)
        {
            await FillInTitleInput(titleToBeEntered);
            await FillInContentInput(contentToBeEntered);
        }

        public async Task WaitForModalToBeRemoved()
        {
            await modalContainer.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Detached });
        }

        public async Task WaitForSuccessMessage()
        {
            await messageSuccess.WaitForAsync();
        }

        public async Task WaitForFailureMessage()
        {
            await messageFailure.WaitForAsync();
        }
    }
}
