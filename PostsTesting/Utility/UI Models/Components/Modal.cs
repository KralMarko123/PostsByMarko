using Microsoft.Playwright;
using Xunit;

namespace PostsTesting.Utility.Pages
{
    public class Modal
    {

        private IPage page;
        public Modal(IPage page) => this.page = page;


        public ILocator modal => page.Locator(".modal");
        public ILocator title => page.Locator(".modal__title");
        public ILocator titleInput => page.Locator("#title");
        public ILocator contentInput => page.Locator("#content");
        public ILocator submitButton => page.Locator(".form__actions .button", new PageLocatorOptions { HasTextString = "Submit" });
        public ILocator cancelButton => page.Locator(".form__actions .button", new PageLocatorOptions { HasTextString = "Cancel" });
        public ILocator deleteButton => page.Locator(".form__actions .button", new PageLocatorOptions { HasTextString = "Delete" });
        public ILocator messageFailure => page.Locator(".modal__message.fail");
        public ILocator messageSuccess => page.Locator(".modal__message.success");


        public async Task CloseModal()
        {
            await ClickCancel();
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

        public async Task ClickSubmit()
        {
            await submitButton.ClickAsync();
        }

        public async Task ClickCancel()
        {
            await cancelButton.ClickAsync();
        }
        public async Task ClickDelete(string expectedMessage = null)
        {
            await deleteButton.ClickAsync();
            if (expectedMessage != null) await CheckSuccessMessage(expectedMessage);
            await WaitForModalToBeRemoved();
        }

        public async Task FillInFormAndSubmit(string titleToBeEntered, string contentToBeEntered, string expectedMessage = null)
        {
            await FillInTitleInput(titleToBeEntered);
            await FillInContentInput(contentToBeEntered);
            await ClickSubmit();
            if (expectedMessage != null) await CheckSuccessMessage(expectedMessage);
            await WaitForModalToBeRemoved();
        }

        public async Task CheckVisibility(string expectedTitleText)
        {
            bool modalIsDisplayed = await modal.IsVisibleAsync() && await title.IsVisibleAsync();
            string titleText = await title.TextContentAsync();

            Assert.True(modalIsDisplayed);
            Assert.Equal(expectedTitleText, titleText);
        }

        public async Task CheckSuccessMessage(string expectedSuccessMessage)
        {
            await WaitForSuccessMessage();

            var successMessage = await messageSuccess.TextContentAsync();
            Assert.Equal(expectedSuccessMessage, successMessage);
        }

        public async Task CheckFailureMessage(string expectedErrorMessage)
        {
            await WaitForFailureMessage();

            var errorMessage = await messageFailure.TextContentAsync();
            Assert.Equal(expectedErrorMessage, errorMessage);
        }

        public async Task WaitForModalToBeRemoved()
        {
            await modal.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Detached });
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
