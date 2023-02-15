using FluentAssertions;
using Microsoft.Playwright;
using PostsTesting.Utility;
using PostsTesting.Utility.Pages;
using PostsTesting.Utility.UI_Models.Pages;
using Xunit;

namespace PostsTesting.Tests.Frontend.Base
{
    public class PostsUiTestBase : UiTestBase, IAsyncLifetime
    {
        HomePage homePage => new HomePage(page);
        PostDetailsPage postDetailsPage => new PostDetailsPage(page);

        public async new Task InitializeAsync()
        {
            await base.InitializeAsync();
            await LoginAsTestUser();
        }

        public async Task VerifyPostDetailsForEachPost()
        {
            var postsArePresent = await homePage.WaitForPostsToLoad();

            if (postsArePresent)
            {
                var postCount = await homePage.GetNumberOfPosts();
                for (int i = 0; i < postCount; i++)
                {
                    var post = new Post(page, homePage.postCard.Nth(i));
                    var postTitle = await post.title.TextContentAsync();
                    var postContent = await post.content.TextContentAsync();

                    await post.ClickOnPost();
                    await postDetailsPage.CheckPostDetails(postTitle, postContent);
                    await postDetailsPage.ClickOnBackButton();
                }
            }
        }

        public async Task VerifyPostDetailsForNotFoundPost()
        {
            await postDetailsPage.Visit("404");
            await postDetailsPage.CheckPostDetails("Cannot open post", "Post with Id: 404 was not found");
        }

        public async Task VerifyPostCanBeCreated()
        {
            var randomTitle = RandomDataGenerator.GetRandomTextWithLength(10);
            var randomContent = RandomDataGenerator.GetRandomTextWithLength(30);
            var newlyCreatedPost = await CreateANewPost(randomTitle, randomContent);

            await newlyCreatedPost.CheckPostState();
            await newlyCreatedPost.CheckPostTitleAndContent(randomTitle, randomContent);
        }

        public async Task VerifyPostCanBeUpdated()
        {
            var randomTitle = RandomDataGenerator.GetRandomTextWithLength(10);
            var randomContent = RandomDataGenerator.GetRandomTextWithLength(30);
            var newlyCreatedPost = await CreateANewPost(randomTitle, randomContent);

            await newlyCreatedPost.ClickOnUpdateIcon();
            await newlyCreatedPost.modal.SubmitModal(shouldCloseModal: false);
            await newlyCreatedPost.modal.CheckFailureMessage("Can't update with same data");

            randomTitle = RandomDataGenerator.GetRandomTextWithLength(10);
            randomContent = RandomDataGenerator.GetRandomTextWithLength(30);

            await newlyCreatedPost.modal.FillInFormAndSubmit(randomTitle, randomContent, "Post was updated successfully");

            var updatedPost = homePage.GetPostWithTitle(randomTitle);
            await updatedPost.CheckPostState();
            await updatedPost.CheckPostTitleAndContent(randomTitle, randomContent);
        }

        public async Task VerifyPostCanBeDeleted()
        {
            var randomTitle = RandomDataGenerator.GetRandomTextWithLength(10);
            var randomContent = RandomDataGenerator.GetRandomTextWithLength(30);
            var newlyCreatedPost = await CreateANewPost(randomTitle, randomContent);
            var numberOfPostsPriorDelete = await homePage.GetNumberOfPosts();

            await newlyCreatedPost.ClickOnDeleteIcon();
            await newlyCreatedPost.modal.SubmitModal(expectedMessage: "Post was deleted successfully");

            var isPostVisible = await page.Locator(".post", new PageLocatorOptions { HasTextString = randomTitle }).IsVisibleAsync();
            isPostVisible.Should().BeFalse();

            var numberOfPostsAfterDelete = await homePage.GetNumberOfPosts();
            numberOfPostsAfterDelete.Should().Be(numberOfPostsPriorDelete - 1);
        }

        public async Task VerifyPostCanBeHidden()
        {
            var randomTitle = RandomDataGenerator.GetRandomTextWithLength(10);
            var randomContent = RandomDataGenerator.GetRandomTextWithLength(30);
            var newlyCreatedPost = await CreateANewPost(randomTitle, randomContent);

            await newlyCreatedPost.ClickOnHideICon();

            var isPostVisible = await page.Locator(".post.hidden", new PageLocatorOptions { HasTextString = randomTitle }).IsVisibleAsync();
            isPostVisible.Should().BeFalse();
        }

        public async Task VerifyPostFiltersCanBeChecked()
        {
            var randomTitle = RandomDataGenerator.GetRandomTextWithLength(10);
            var randomContent = RandomDataGenerator.GetRandomTextWithLength(30);
            var newlyCreatedPost = await CreateANewPost(randomTitle, randomContent);

            await homePage.ToggleMyPostsCheckbox();
            await homePage.ToggleMyPostsCheckbox(false);
            await newlyCreatedPost.ClickOnHideICon();
            await homePage.ToggleHiddenPostsCheckbox();

            var isPostVisible = await page.Locator(".post.hidden", new PageLocatorOptions { HasTextString = randomTitle }).IsVisibleAsync();
            isPostVisible.Should().BeTrue();

            await homePage.ToggleHiddenPostsCheckbox(false);
            isPostVisible = await page.Locator(".post.hidden", new PageLocatorOptions { HasTextString = randomTitle }).IsVisibleAsync();
            isPostVisible.Should().BeFalse();
        }

        public async Task VerifyPostAllowedUsersCanBeModified()
        {
            var randomTitle = RandomDataGenerator.GetRandomTextWithLength(10);
            var randomContent = RandomDataGenerator.GetRandomTextWithLength(30);
            var newlyCreatedPost = await CreateANewPost(randomTitle, randomContent);

            await newlyCreatedPost.ClickOnPost();
            await postDetailsPage.ClickOnToggleUsersButton();
            await postDetailsPage.modal.CheckVisibility("Toggle User For Post");
            await postDetailsPage.modal.SubmitModal(shouldCloseModal: false);
            await postDetailsPage.modal.CheckFailureMessage("Please select a user");

            var selectedValueText = await postDetailsPage.select.value.TextContentAsync();
            selectedValueText.Should().BeNullOrEmpty();

            await postDetailsPage.select.OpenSelect();

            var optionToBeSelected = await postDetailsPage.select.GetOptionContainingText(adminUser.UserName).TextContentAsync();
            optionToBeSelected.Should().Contain("Hidden");

            await postDetailsPage.select.ClickOnOption(adminUser.UserName);
            await postDetailsPage.modal.SubmitModal(expectedMessage: "User was toggled successfully");
            await postDetailsPage.ClickOnToggleUsersButton();
            await postDetailsPage.select.OpenSelect();

            optionToBeSelected = await postDetailsPage.select.GetOptionContainingText(adminUser.UserName).TextContentAsync();
            optionToBeSelected.Should().Contain("Allowed");
        }
    }
}
