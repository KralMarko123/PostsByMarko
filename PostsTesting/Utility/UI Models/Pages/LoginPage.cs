using FluentAssertions;
using Microsoft.Playwright;

namespace PostsTesting.Utility.UI_Models.Pages
{
    public class LoginPage : Page
    {
        private static string url => $"{baseUrl}/login";
        public LoginPage(IPage page) : base(page) { }


        public ILocator username => page.Locator("#username");
        public ILocator password => page.Locator("#password");
        public ILocator loginButton => button;
        public ILocator registerLink => link;


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
            await errorMessage.WaitForAsync();

            var errorTitleText = await errorMessage.TextContentAsync();
            errorTitleText.Should().Be(expectedErrorTitle);

            if (expectedErrorMessages != null)
            {
                var numberOfErrorMessages = await errorSubmessage.CountAsync();
                for (int i = 0; i < numberOfErrorMessages; i++)
                {
                    var errorMessageText = await errorSubmessage.Nth(i).TextContentAsync();
                    errorMessageText.Should().Be(expectedErrorMessages[i]);
                }
            }
        }
    }
}
