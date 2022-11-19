using PostsTesting.Utility;
using PostsTesting.Utility.Constants;
using PostsTesting.Utility.Models;
using PostsTesting.Utility.UI_Models.Pages;
using Xunit;

namespace PostsTesting.Tests.TestBase
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

            Assert.True(userDisplayed);
            Assert.Contains($"{testUser.FirstName} {testUser.LastName}", userDetails);
        }

        public async Task VerifyUserCanBeRegistered()
        {
            User randomlyGeneratedTestUser = AppConstants.RandomTestUser;

            await registerPage.Visit();
            await registerPage.Register(randomlyGeneratedTestUser.FirstName, randomlyGeneratedTestUser.LastName, randomlyGeneratedTestUser.Username, randomlyGeneratedTestUser.Password);
            await registerPage.loginLink.WaitForAsync();

            var hasSuccessfullyRegistered = await registerPage.successfullyRegisteredLink.IsVisibleAsync();
            var linkText = await registerPage.successfullyRegisteredLink.TextContentAsync();

            Assert.True(hasSuccessfullyRegistered);
            Assert.Equal(linkText, "You have successfully registered. Click here to log in");
        }

        public async Task VerifyErrorMessagesWhenLoggingIn()
        {
            var randomTestText = RandomDataGenerator.GetRandomTextWithLength(10);

            await loginPage.Visit();
            await loginPage.ClickLoginButton();
            await loginPage.CheckForErrors("Password can't be empty");
            await loginPage.FillInPasswordInput(randomTestText);
            await loginPage.ClickLoginButton();
            await loginPage.CheckForErrors("Username can't be empty");
            await loginPage.FillInUsernameInput(randomTestText);
            await loginPage.ClickLoginButton();
            await loginPage.CheckForErrors("Invalid Login, please check your credentials and try again");
        }

        public async Task VerifyErrorMessagesWhenRegistering()
        {
            var randomTestText = RandomDataGenerator.GetRandomTextWithLength(10);

            await registerPage.Visit();
            await registerPage.ClickRegisterButton();
            await registerPage.CheckForErrors("Confirm Password can't be empty");
            await registerPage.FillInConfirmPasswordInput(randomTestText);
            await registerPage.ClickRegisterButton();
            await registerPage.CheckForErrors("Password can't be empty");
            await registerPage.FillInPasswordInput("@");
            await registerPage.ClickRegisterButton();
            await registerPage.CheckForErrors("Username can't be empty");
            await registerPage.FillInUsernameInput(randomTestText);
            await registerPage.ClickRegisterButton();
            await registerPage.CheckForErrors("Last Name can't be empty");
            await registerPage.FillInLastNameInput(randomTestText);
            await registerPage.ClickRegisterButton();
            await registerPage.CheckForErrors("First Name can't be empty");
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
            await registerPage.FillInUsernameInput(testUser.Username);
            await registerPage.ClickRegisterButton();
            await registerPage.CheckForErrors("The username is already taken. Please use a different one");
        }
    }
}
