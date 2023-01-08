using PostsTesting.Tests.Frontend.Base;
using Xunit;

namespace PostsTesting.Tests.Frontend.Posts
{
    public class PostsTests : PostsUiTestBase
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

        [Fact]
        public async Task VerifyPostVisibilityCanBeToggled()
        {
            await VerifyPostCanBeHidden();
        }

        [Fact]
        public async Task VerifyPostFiltersCanBeToggled()
        {
            await VerifyPostFiltersCanBeChecked();
        }

        [Fact]
        public async Task VerifyPostAllowedUserCanBeToggled()
        {
            await VerifyPostAllowedUsersCanBeModified();
        }
    }
}
