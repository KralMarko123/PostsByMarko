using FluentAssertions;
using Microsoft.Playwright;

namespace PostsTesting.UI_Models.Components
{
    public class Modal : Component
    {
        public Modal(IPage page) : base(page) { }


        public ILocator modal => page.Locator(".modal");
        public ILocator title => page.Locator(".modal__title");
        public ILocator titleInput => page.Locator("#title");
        public ILocator contentInput => page.Locator("#content");
        public ILocator messageFailure => page.Locator(".modal__message.fail");
        public ILocator messageSuccess => page.Locator(".modal__message.success");

        public async Task SubmitModal(string buttonText = "Submit", string expectedMessage = null, bool shouldCloseModal = true)
        {
            if (expectedMessage != null) await CheckSuccessMessage(expectedMessage);
            if (shouldCloseModal) await WaitForModalToBeRemoved();
        }

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
            await SubmitModal(expectedMessage: expectedMessage);
        }

        public async Task CheckVisibility(string expectedTitleText)
        {
            var modalIsDisplayed = await modal.IsVisibleAsync() && await title.IsVisibleAsync();
            var titleText = await title.TextContentAsync();

            modalIsDisplayed.Should().BeTrue();
            titleText.Should().Be(expectedTitleText);
        }

        public async Task CheckSuccessMessage(string expectedSuccessMessage)
        {
            await WaitForSuccessMessage();

            var successMessage = await messageSuccess.TextContentAsync();
            successMessage.Should().Be(expectedSuccessMessage);
        }

        public async Task CheckFailureMessage(string expectedErrorMessage)
        {
            await WaitForFailureMessage();

            var errorMessage = await messageFailure.TextContentAsync();
            errorMessage.Should().Be(expectedErrorMessage);
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
