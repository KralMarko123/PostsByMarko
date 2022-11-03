using PostsTesting.Utility.Pages;

namespace PostsTesting.Tests.TestBase
{
    public class UiTestBase : Base
    {
        HomePage homePage => new HomePage(page);
        PostDetailsPage postDetailsPage => new PostDetailsPage(page);

        public async Task VerifyHomepageDefaultState()
        {
            await homePage.Visit();
            await homePage.CheckDefaultState();
        }

        public async Task VerifyPostDetailsForEachPost()
        {
            await homePage.Visit();
            var postsArePresent = await homePage.WaitForPostsToLoad();

            if (postsArePresent)
            {
                var postCount = await homePage.postCard.CountAsync();
                for (int i = 0; i < postCount; i++)
                {
                    Post post = new Post(page, homePage.postCard.Nth(i));
                    var postTitle = await post.title.TextContentAsync();
                    var postContent = await post.content.TextContentAsync();

                    await post.ClickOnPost();
                    await postDetailsPage.CheckPostDetails(postTitle, postContent);
                    await postDetailsPage.GoBack();
                }
            }
        }

        public async Task VerifyPostDetailsForNotFoundPost()
        {
            await postDetailsPage.Visit("404");
            await postDetailsPage.CheckPostDetails("No Post Found", "The post with Id: 404 doesn't seem to exist. Go back to view other posts.");
        }
    }
}
