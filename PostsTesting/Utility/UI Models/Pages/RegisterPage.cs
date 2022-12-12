using FluentAssertions;
using Microsoft.Playwright;

namespace PostsTesting.Utility.UI_Models.Pages
{
    public class RegisterPage : Page
    {
        private static string url => $"{baseUrl}/register";
        public RegisterPage(IPage page) : base(page) { }


        public ILocator firstName => page.Locator("#firstName");
        public ILocator lastName => page.Locator("#lastName");
        public ILocator username => page.Locator("#username");
        public ILocator password => page.Locator("#password");
        public ILocator confirmPassword => page.Locator("#confirmPassword");
        public ILocator registerButton => button;
        public ILocator loginLink => link;


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

        public async Task CheckForSuccessfulRegistration()
        {
            await title.WaitForAsync();

            var titleText = await title.TextContentAsync();
            var descriptionText = await description.TextContentAsync();

            titleText.Should().Be("Successfully Registered!");
            descriptionText.Should().Be("Please check your email to confirm your account first. You can click on the button below to login");
        }

    }
}
