using PostsTesting.Tests.TestBase;
using Xunit;

namespace PostsTesting.Tests.Frontend
{
    public class UiTests : UiTestBase
    {
        [Fact]
        public async Task VerifyHomepageIsDisplayedCorrectly()
        {
            await VerifyHomepageDefaultState();
        }
    }
}