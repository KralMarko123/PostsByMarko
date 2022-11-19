using PostsTesting.Utility.UI_Models.Pages;
using Xunit;

namespace PostsTesting.Tests.TestBase
{
    public class UiTestBase : Base, IAsyncLifetime
    {
        LoginPage loginPage => new LoginPage(page);
        HomePage homePage => new HomePage(page);

        public async Task InitializeAsync() => await base.InitializeAsync();

        public async Task LoginAsTestUser()
        {
            await loginPage.Visit();
            await loginPage.Login(testUser.Username, testUser.Password);
            await homePage.home.WaitForAsync();
        }

        public async Task VerifyHomepageDefaultState()
        {
            await LoginAsTestUser();
            await homePage.CheckDefaultState();
        }
    }
}
