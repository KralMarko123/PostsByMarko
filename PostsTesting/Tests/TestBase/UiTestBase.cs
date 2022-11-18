using Microsoft.Playwright;
using PostsTesting.Utility;
using PostsTesting.Utility.Constants;
using PostsTesting.Utility.Models;
using PostsTesting.Utility.Pages;
using PostsTesting.Utility.UI_Models.Pages;
using System.Net;
using Xunit;

namespace PostsTesting.Tests.TestBase
{
    public class UiTestBase : Base
    {
        RegisterPage registerPage => new RegisterPage(page);
        HomePage homePage => new HomePage(page);
        PostDetailsPage postDetailsPage => new PostDetailsPage(page);


        public async Task VerifyUserCanBeRegistered()
        {
            User testUser = AppConstants.TestUser;

            await registerPage.Visit();
            await registerPage.Register(testUser.FirstName, testUser.LastName, testUser.Username, testUser.Password);
            await registerPage.loginLink.WaitForAsync();

            var hasSuccessfullyRegistered = await registerPage.successfullyRegisteredLink.IsVisibleAsync();
            var linkText = await registerPage.successfullyRegisteredLink.TextContentAsync();

            Assert.True(hasSuccessfullyRegistered);
            Assert.Equal(linkText, "You have successfully registered. Click here to log in");
        }

        public async Task VerifyErrorMessagesWhenRegistering()
        {
            var randomTestText = RandomDataGenerator.GetRandomTextWithLength(10);

            await registerPage.Visit();
            await registerPage.ClickRegisterButton();
            await registerPage.CheckForErrors("Confirm Password can't be empty");
            await registerPage.FillInConfirmPasswordInput(randomTestText);
            await registerPage.ClickRegisterButton();
            await registerPage.CheckForErrors("Password can't be empty");
            await registerPage.FillInPasswordInput("@");
            await registerPage.ClickRegisterButton();
            await registerPage.CheckForErrors("Username can't be empty");
            await registerPage.FillInUsernameInput(randomTestText);
            await registerPage.ClickRegisterButton();
            await registerPage.CheckForErrors("Last Name can't be empty");
            await registerPage.FillInLastNameInput(randomTestText);
            await registerPage.ClickRegisterButton();
            await registerPage.CheckForErrors("First Name can't be empty");
            await registerPage.FillInFirstNameInput(randomTestText);
            await registerPage.ClickRegisterButton();
            await registerPage.CheckForErrors("Invalid Username", new List<string> { "Username should be a valid email address" });
            await registerPage.FillInUsernameInput($"{randomTestText}@{randomTestText}.com");
            await registerPage.ClickRegisterButton();
            await registerPage.CheckForErrors("Password does not meet the requirements", new List<string>
            {
                "Should be at least six characters long",
                "Have one lowercase letter",
                "Have one uppercase letter",
                "Have one digit"
            });
            await registerPage.FillInPasswordInput($"{randomTestText}test123");
            await registerPage.ClickRegisterButton();
            await registerPage.CheckForErrors("Passwords do not match");
        }

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
            IResponse notFoundResponse = await postDetailsPage.GetResponseForPostDetails("404");
            Assert.True(notFoundResponse.Status == (int)HttpStatusCode.NotFound);

            await postDetailsPage.CheckPostDetails("No Post Found", "The post with Id: 404 doesn't seem to exist. Go back to view other posts.");
        }

        public async Task VerifyPostCanBeCreated()
        {
            var randomTitle = RandomDataGenerator.GetRandomTextWithLength(10);
            var randomContent = RandomDataGenerator.GetRandomTextWithLength(30);

            await homePage.Visit();
            await homePage.ClickCreatePostButton();
            await homePage.modal.ClickSubmit();
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
