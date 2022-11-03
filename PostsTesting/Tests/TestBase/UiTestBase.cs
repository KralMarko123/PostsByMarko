using Microsoft.Playwright;
using PostsTesting.Utility;
using PostsTesting.Utility.Pages;
using Xunit;

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
                var postCount = await homePage.GetNumberOfPosts();
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

        public async Task VerifyPostCanBeCreated()
        {
            var randomTitle = RandomDataGenerator.GetRandomTextWithLength(10);
            var randomContent = RandomDataGenerator.GetRandomTextWithLength(30);

            await homePage.Visit();
            await homePage.ClickCreatePostButton();
            await homePage.modal.ClickSubmit();
            await homePage.modal.CheckPlaceholderErrorMessages();
            await homePage.modal.FillInFormAndSubmit(randomTitle, randomContent, "Post created successfully.");

            var newlyCreatedPost = homePage.FindPostWithTitleAndContent(randomTitle);

            await newlyCreatedPost.CheckPost();
            await newlyCreatedPost.CheckPostTitleAndContent(randomTitle, randomContent);
        }

        public async Task VerifyPostCanBeUpdated()
        {
            var randomTitle = RandomDataGenerator.GetRandomTextWithLength(10);
            var randomContent = RandomDataGenerator.GetRandomTextWithLength(30);

            await homePage.Visit();
            await homePage.ClickCreatePostButton();
            await homePage.modal.FillInFormAndSubmit(randomTitle, randomContent);

            var newlyCreatedPost = homePage.FindPostWithTitleAndContent(randomTitle);

            await newlyCreatedPost.ClickOnUpdateIcon();
            await newlyCreatedPost.modal.ClickSubmit();
            await newlyCreatedPost.modal.CheckFailureMessage("Can't update with same data.");

            randomTitle = RandomDataGenerator.GetRandomTextWithLength(10);
            randomContent = RandomDataGenerator.GetRandomTextWithLength(30);

            await newlyCreatedPost.modal.FillInFormAndSubmit(randomTitle, randomContent, "Post updated successfully.");

            var updatedPost = homePage.FindPostWithTitleAndContent(randomTitle);

            await updatedPost.CheckPost();
            await updatedPost.CheckPostTitleAndContent(randomTitle, randomContent);
        }

        public async Task VerifyPostCanBeDeleted()
        {
            var randomTitle = RandomDataGenerator.GetRandomTextWithLength(10);
            var randomContent = RandomDataGenerator.GetRandomTextWithLength(30);

            await homePage.Visit();
            await homePage.ClickCreatePostButton();
            await homePage.modal.FillInFormAndSubmit(randomTitle, randomContent);

            var numberOfPostsPriorDelete = await homePage.GetNumberOfPosts();
            var newlyCreatedPost = homePage.FindPostWithTitleAndContent(randomTitle);

            await newlyCreatedPost.ClickOnDeleteIcon();
            await newlyCreatedPost.modal.ClickDelete("Post deleted successfully.");

            var isPostVisible = await page.Locator(".post", new PageLocatorOptions { HasTextString = randomTitle }).IsVisibleAsync();
            Assert.False(isPostVisible);

            var numberOfPostsAfterDelete = await homePage.GetNumberOfPosts();
            Assert.Equal(numberOfPostsAfterDelete, numberOfPostsPriorDelete - 1);
        }
    }
}
