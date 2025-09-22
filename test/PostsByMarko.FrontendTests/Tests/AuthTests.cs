using FluentAssertions;
using Microsoft.Playwright;
using PostsByMarko.FrontendTests.Tests;
using PostsByMarko.Shared.Constants;
using PostsByMarko.Test.Shared.Helper;
using PostsTesting.UI_Models.Pages;
using Xunit;
using static Microsoft.Playwright.Assertions;

namespace PostsByMarko.FrontendTests.Frontend
{
    [Collection("Frontend Collection")]
    public class AuthTests
    {
        private IBrowser browser;
        private IPage page;
        private HomePage homePage => new HomePage(page);
        private LoginPage loginPage => new LoginPage(page);
        private RegisterPage registerPage => new RegisterPage(page);

        public AuthTests(PostsByMarkoFactory postsByMarkoHostFactory)
        {
            browser = postsByMarkoHostFactory.driver.GetFirefoxBrowserAsync().Result;
            page = browser.NewPageAsync().Result;
        }

        [Fact]
        public async Task should_show_home_page_after_login()
        {
            await loginPage.Visit();
            await loginPage.Login(TestingConstants.TEST_USER.Email, TestingConstants.TEST_PASSWORD);
            await Expect(homePage.home).ToBeVisibleAsync();

            var homePageTitleText = await homePage.containerTitle.TextContentAsync();

            homePageTitleText.Should().Be("Today's Posts");
        }

        [Fact]
        public async Task should_show_confirmational_box_after_registering()
        {
            await registerPage.Visit();
            await registerPage.Register("Test", "User", $"test_{RandomHelper.GetRandomString(5)}@domain.com", "@Test123");
            await registerPage.confirmationalForm.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

            var successfulRegisterText = await registerPage.formTitle.TextContentAsync();
            successfulRegisterText.Should().Be("Successfully Registered!");
        }

        public async Task DisposeAsync()
        {
            await page.CloseAsync();
            await browser.CloseAsync();
        }
    }
}
