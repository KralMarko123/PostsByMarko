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

        [Fact]
        public async Task VerifyPostDetailsPageIsDisplayedCorrectlyForEachPost()
        {
            await VerifyPostDetailsForEachPost();
        }

        [Fact]
        public async Task VerifyPostDetailsPageForNonExistentPost()
        {
            await VerifyPostDetailsForNotFoundPost();
        }

        [Fact]
        public async Task VerifyNewlyCreatedPostIsPresentOnHomePage()
        { 
            await VerifyPostCanBeCreated();
        }

        [Fact]
        public async Task VerifyPostCanBeEditedFromTheHomepage()
        {
            await VerifyPostCanBeUpdated();
        }

        [Fact]
        public async Task VerifyPostIsNotPresentAfterBeingDeleted()
        {
            await VerifyPostCanBeDeleted();
        }
    }
}