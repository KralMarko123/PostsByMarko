using Microsoft.Playwright;
using PostsTesting.Utility.Constants;
using Xunit;

namespace PostsTesting.Utility.UI_Models.Pages
{
    public class RegisterPage
    {
        private IPage page;
        private static string url => $"{AppConstants.UiEndpoint}/register";
        public RegisterPage(IPage page) => this.page = page;


        public ILocator firstName => page.Locator("#firstName");
        public ILocator lastName => page.Locator("#lastName");
        public ILocator username => page.Locator("#username");
        public ILocator password => page.Locator("#password");
        public ILocator confirmPassword => page.Locator("#confirmPassword");
        public ILocator registerButton => page.Locator(".button");
        public ILocator loginLink => page.Locator(".link");
        public ILocator errorTitle => page.Locator(".error");
        public ILocator errorMessage => page.Locator(".error__message");
        public ILocator successfullyRegisteredLink => page.Locator(".link.success");


        public async Task Visit()
        {
            await page.GotoAsync(url);
        }

        public async Task ClickRegisterButton()
        {
            await registerButton.ClickAsync();
        }

        public async Task ClickLoginLink()
        {
            await loginLink.ClickAsync();
        }

        public async Task FillInFirstNameInput(string firstNameToBeEntered)
        {
            await firstName.FillAsync(firstNameToBeEntered);
        }
        public async Task FillInLastNameInput(string lastNameToBeEntered)
        {
            await lastName.FillAsync(lastNameToBeEntered);
        }

        public async Task FillInUsernameInput(string usernameToBeEntered)
        {
            await username.FillAsync(usernameToBeEntered);
        }

        public async Task FillInPasswordInput(string passwordToBeEntered)
        {
            await password.FillAsync(passwordToBeEntered);
        }

        public async Task FillInConfirmPasswordInput(string passwordToBeEntered)
        {
            await confirmPassword.FillAsync(passwordToBeEntered);
        }

        public async Task Register(string firstName, string lastName, string username, string password)
        {
            await FillInFirstNameInput(firstName);
            await FillInLastNameInput(lastName);
            await FillInUsernameInput(username);
            await FillInPasswordInput(password);
            await FillInConfirmPasswordInput(password);
            await ClickRegisterButton();
        }

        public async Task CheckForErrors(string expectedErrorTitle, List<string> expectedErrorMessages = null)
        {
            await errorTitle.WaitForAsync();

            var errorTitleText = await errorTitle.TextContentAsync();
            Assert.Equal(errorTitleText, expectedErrorTitle);

            if (expectedErrorMessages != null)
            {
                var numberOfErrorMessages = await errorMessage.CountAsync();
                for (int i = 0; i < numberOfErrorMessages; i++)
                {
                    var errorMessageText = await errorMessage.Nth(i).TextContentAsync();
                    Assert.Equal(errorMessageText, expectedErrorMessages.ElementAt(i));
                }
            }
        }

    }
}
