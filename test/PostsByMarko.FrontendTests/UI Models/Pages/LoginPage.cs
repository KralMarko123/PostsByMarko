using FluentAssertions;
using Microsoft.Playwright;

namespace PostsTesting.UI_Models.Pages
{
    public class LoginPage : Page
    {
        private static string url => $"{baseUrl}/login";
        public LoginPage(IPage page) : base(page) { }

        public ILocator email => page.Locator("#email");
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
            await email.FillAsync(usernameToBeEntered);
        }

        public async Task FillInPasswordInput(string passwordToBeEntered)
        {
            await password.FillAsync(passwordToBeEntered);
        }

        public async Task Login(string email, string password)
        {
            await FillInUsernameInput(email);
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
