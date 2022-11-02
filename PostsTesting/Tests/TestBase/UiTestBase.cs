using Microsoft.VisualStudio.TestPlatform.ObjectModel.Engine.ClientProtocol;
using PostsTesting.Utility.Pages;

namespace PostsTesting.Tests.TestBase
{
    public class UiTestBase : Base
    {
        HomePage homePage => new HomePage(page);

        public async Task VerifyHomepageDefaultState()
        {
            await homePage.Visit();
            await homePage.title.IsVisibleAsync();
            Thread.Sleep(5000);
        }
    }
}
