using PostsTesting.Utility.Pages;
using Xunit;

namespace PostsTesting.Tests.TestBase
{
    public class UiTestBase : Base
    {
        HomePage homePage => new HomePage(page);

        public async Task VerifyHomepageDefaultState()
        {
            await homePage.Visit();
            await homePage.CheckDefaultState();
        }
    }
}
