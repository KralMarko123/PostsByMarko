using PostsTesting.Utility.Pages;
using PostsTesting.Utility.UI_Models.Pages;
using Xunit;

namespace PostsTesting.Tests.Frontend.Base
{
    public class UiTestBase : FrontendBase, IAsyncLifetime
    {
        LoginPage loginPage => new LoginPage(page);
        HomePage homePage => new HomePage(page);

        public new async Task InitializeAsync() => await base.InitializeAsync();

        public async Task LoginAsTestUser()
        {
            await loginPage.Visit();
            await loginPage.Login(testUser.UserName, "Test123");
            await homePage.home.WaitForAsync();
        }

        public async Task VerifyHomepageDefaultState()
        {
            await LoginAsTestUser();
            await homePage.CheckDefaultState();
        }

        public async Task<Post> CreateANewPost(string title, string content)
        {
            await homePage.Visit();
            await homePage.ClickCreatePostButton();
            await homePage.modal.FillInFormAndSubmit(title, content, "Post was created successfully");

            return homePage.GetPostWithTitle(title);
        }
    }
}
