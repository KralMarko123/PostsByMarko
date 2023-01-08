using FluentAssertions;
using PostsTesting.Utility;
using PostsTesting.Utility.UI_Models.Pages;
using Xunit;

namespace PostsTesting.Tests.Frontend.Base
{
    public class AuthUiTestBase : UiTestBase, IAsyncLifetime
    {
        RegisterPage registerPage => new RegisterPage(page);
        LoginPage loginPage => new LoginPage(page);
        HomePage homePage => new HomePage(page);


        public async Task InitializeAsync() => await base.InitializeAsync();

        public async Task VerifyUserCanBeLoggedIn()
        {
            await LoginAsTestUser();
            var userDisplayed = await homePage.username.IsVisibleAsync();
            var userDetails = await homePage.username.TextContentAsync();

            userDisplayed.Should().BeTrue();
            userDetails.Should().Contain($"{testUser.FirstName} {testUser.LastName}");
        }

        public async Task VerifyUserCanBeRegistered()
        {
            var randomlyGeneratedTestUser = RandomDataGenerator.GetRandomTestUser();

            await registerPage.Visit();
            await registerPage.Register(randomlyGeneratedTestUser.FirstName, randomlyGeneratedTestUser.LastName, randomlyGeneratedTestUser.UserName, "Random123");
            await registerPage.CheckForSuccessfulRegistration();
        }

        public async Task VerifyErrorMessagesWhenLoggingIn()
        {
            var randomTestText = RandomDataGenerator.GetRandomTextWithLength(10);

            await loginPage.Visit();
            await loginPage.ClickLoginButton();
            await loginPage.CheckForErrors("Fields can't be empty");
            await loginPage.FillInPasswordInput(randomTestText);
            await loginPage.FillInUsernameInput(randomTestText);
            await loginPage.ClickLoginButton();
            await loginPage.CheckForErrors("No account found, please check your credentials and try again");
        }

        public async Task VerifyErrorMessagesWhenRegistering()
        {
            var randomTestText = RandomDataGenerator.GetRandomTextWithLength(10);

            await registerPage.Visit();
            await registerPage.ClickRegisterButton();
            await registerPage.CheckForErrors("Fields can't be empty");
            await registerPage.FillInConfirmPasswordInput(randomTestText);
            await registerPage.FillInPasswordInput("@");
            await registerPage.FillInUsernameInput(randomTestText);
            await registerPage.FillInLastNameInput(randomTestText);
            await registerPage.FillInFirstNameInput(randomTestText);
            await registerPage.ClickRegisterButton();
            await registerPage.CheckForErrors("Invalid Username", new List<string> { "Username should be a valid email address" });
            await registerPage.FillInUsernameInput($"{randomTestText}@{randomTestText}.com");
            await registerPage.ClickRegisterButton();
            await registerPage.CheckForErrors("Password does not meet the requirements", new List<string>
            {
                "Should be at least six characters long",
                "Have one lowercase letter",
                "Have one uppercase letter",
                "Have one digit"
            });
            await registerPage.FillInPasswordInput($"{randomTestText}test123");
            await registerPage.ClickRegisterButton();
            await registerPage.CheckForErrors("Passwords do not match");
            await registerPage.FillInConfirmPasswordInput($"{randomTestText}test123");
            await registerPage.FillInUsernameInput(testUser.UserName);
            await registerPage.ClickRegisterButton();
            await registerPage.CheckForErrors($"Username '{testUser.UserName}' is already taken.");
        }
    }
}
