using FluentAssertions;
using Microsoft.Playwright;
using PostsByMarko.Test.Shared.Constants;
using PostsByMarko.Test.Shared.Helper;
using PostsTesting.UI_Models.Pages;
using Xunit;

namespace PostsByMarko.FrontendTests.Tests
{
    [Collection("Frontend Collection")]
    public class AuthTests : IAsyncLifetime
    {
        private readonly PostsByMarkoFactory postsByMarkoFactory;
        private IPage page;
        private HomePage homePage;
        private LoginPage loginPage;
        private RegisterPage registerPage;
        private readonly string testUserEmail = TestingConstants.TEST_USER_EMAIL;

        public AuthTests(PostsByMarkoFactory postsByMarkoFactory)
        {
            this.postsByMarkoFactory = postsByMarkoFactory;
        }

        // Setup
        public async Task InitializeAsync()
        {
            page = await postsByMarkoFactory.browser.NewPageAsync();

            homePage = new HomePage(page);
            loginPage = new LoginPage(page);
            registerPage = new RegisterPage(page);
        }

        // Teardown
        public async Task DisposeAsync()
        {
            if (page != null) await page.CloseAsync();
        }

        [Fact]
        public async Task should_login()
        {
            await LoginWithEmail(testUserEmail);

            var homePageTitleText = await homePage.containerTitle.TextContentAsync();

            homePageTitleText.Should().Be("Today's Posts");
        }

        [Fact]
        public async Task should_register()
        {
            await registerPage.Visit();
            await registerPage.Register("Test", "User", $"test_{RandomHelper.GetRandomString(5)}@domain.com", "@Test123");
            await registerPage.confirmationalForm.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

            var successfulRegisterText = await registerPage.formTitle.TextContentAsync();
            successfulRegisterText.Should().Be("Successfully Registered!");
        }

        [Fact]
        public async Task should_logout()
        {
            await LoginWithEmail(testUserEmail);

            await homePage.navComponent.dropdownMenu.HoverAsync();
            await homePage.navComponent.logout.ClickAsync();
            await loginPage.loginButton.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        }

        private async Task LoginWithEmail(string email)
        {
            await loginPage.Visit();
            await loginPage.Login(email, TestingConstants.TEST_PASSWORD);
            await homePage.username.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        }
    }
}
