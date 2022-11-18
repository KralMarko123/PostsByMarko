using PostsTesting.Tests.TestBase;
using Xunit;

namespace PostsTesting.Tests.Frontend.Posts
{
    public class PostsTests : UiTestBase
    {
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
