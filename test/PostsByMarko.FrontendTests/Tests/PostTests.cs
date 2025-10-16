using Bogus;
using FluentAssertions;
using Microsoft.Playwright;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Test.Shared.Constants;
using PostsTesting.UI_Models.Pages;
using Xunit;
using Post = PostsTesting.UI_Models.Components.Post;

namespace PostsByMarko.FrontendTests.Tests
{
    [Collection("Frontend Collection")]
    public class PostTests
    {
        private IBrowser browser;
        private IPage page;
        private HomePage homePage => new HomePage(page);
        private LoginPage loginPage => new LoginPage(page);
        private readonly User testUser = TestingConstants.TEST_USER;
        private readonly User testAdmin = TestingConstants.TEST_ADMIN;

        public PostTests(PostsByMarkoFactory postsByMarkoHostFactory)
        {
            browser = postsByMarkoHostFactory.driver.GetFirefoxBrowserAsync().Result;
            page = browser.NewPageAsync().Result;
        }

        [Fact]
        public async Task should_create_post()
        {
            await LoginWithUser(testUser);

            var expectedTitle = new Faker().Commerce.Product();
            var expectedContent = new Faker().Commerce.ProductDescription();

            await homePage.navComponent.dropdownMenu.HoverAsync();
            await homePage.navComponent.createPost.ClickAsync();
            await homePage.modalComponent.FillInTitleInput(expectedTitle);
            await homePage.modalComponent.FillInContentInput(expectedContent);
            await homePage.modalComponent.createButton.ClickAsync();
            await homePage.WaitForPostListSizeToChange();

            var createdPost = new Post(page, homePage.postCard.First);
            var title = await createdPost.title.TextContentAsync();
            var content = await createdPost.content.TextContentAsync();

            title.Should().Be(expectedTitle);
            content.Should().Be(expectedContent);
        }

        [Fact]
        public async Task should_update_a_post()
        {
            await LoginWithUser(testAdmin);

            var post = new Post(page, homePage.postCard.First);

            var newTitle = new Faker().Commerce.Product();
            var newContent = new Faker().Commerce.ProductDescription();

            var oldPostTitle = await post.title.TextContentAsync();
            var oldPostContent = await post.content.TextContentAsync();

            await post.ClickOnUpdateIcon();
            await homePage.modalComponent.FillInTitleInput(newTitle);
            await homePage.modalComponent.FillInContentInput(newContent);
            await homePage.modalComponent.updateButton.ClickAsync();
            await post.WaitForPostContentsToChange();

            var updatedTitle = await post.title.TextContentAsync();
            var updatedContent = await post.content.TextContentAsync();

            updatedTitle.Should().NotBe(oldPostTitle);
            updatedTitle.Should().Be(newTitle);
            updatedContent.Should().NotBe(oldPostContent);
            updatedContent.Should().Be(newContent);
        }

        [Fact]
        public async Task should_delete_a_post()
        {
            await LoginWithUser(testAdmin);

            var post = new Post(page, homePage.postCard.First);

            await post.ClickOnDeleteIcon();
            await homePage.modalComponent.deleteButton.ClickAsync();
            await homePage.WaitForPostListSizeToChange();

            var postWithIdCount = await homePage.postCard.Locator($"#{post.Id}").CountAsync();

            postWithIdCount.Should().Be(0);
        }

        [Fact]
        public async Task should_hide_a_post()
        {
            await LoginWithUser(testAdmin);

            var visiblePost = new Post(page, homePage.page.Locator(".post:not(.hidden)").First);

            await visiblePost.ClickOnHideIcon();
            await visiblePost.WaitForPostVisibilityToToggle();

            var postClassnames = await visiblePost.post.GetAttributeAsync("class");
            
            postClassnames.Should().Contain("hidden");
        }

        private async Task LoginWithUser(User user)
        {
            await loginPage.Visit();
            await loginPage.Login(user.Email, TestingConstants.TEST_PASSWORD);
            await homePage.username.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        }

        public async Task DisposeAsync()
        {
            await page.CloseAsync();
            await browser.CloseAsync();
        }
    }
}
