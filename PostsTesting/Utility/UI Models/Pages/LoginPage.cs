using Microsoft.Playwright;
using PostsTesting.Utility.Constants;
using Xunit;

namespace PostsTesting.Utility.UI_Models.Pages
{
    public class LoginPage
    {
        private IPage page;
        private static string url => $"{AppConstants.UiEndpoint}/login";
        public LoginPage(IPage page) => this.page = page;


        public ILocator username => page.Locator("#username");
        public ILocator password => page.Locator("#password");
        public ILocator loginButton => page.Locator(".button");
        public ILocator registerLink => page.Locator(".link");
        public ILocator errorTitle => page.Locator(".error");
        public ILocator errorMessage => page.Locator(".error__message");


        public async Task Visit()
        {
            await page.GotoAsync(url);
        }

        public async Task ClickLoginButton()
        {
            await loginButton.ClickAsync();
        }

        public async Task ClickRegisterLink()
        {
            await registerLink.ClickAsync();
        }

        public async Task FillInUsernameInput(string usernameToBeEntered)
        {
            await username.FillAsync(usernameToBeEntered);
        }

        public async Task FillInPasswordInput(string passwordToBeEntered)
        {
            await password.FillAsync(passwordToBeEntered);
        }

        public async Task Login(string username, string password)
        {
            await FillInUsernameInput(username);
            await FillInPasswordInput(password);
            await ClickLoginButton();
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
