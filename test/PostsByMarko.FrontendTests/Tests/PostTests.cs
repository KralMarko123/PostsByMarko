using Bogus;
using FluentAssertions;
using Microsoft.Playwright;
using PostsByMarko.Test.Shared.Constants;
using PostsTesting.UI_Models.Pages;
using Xunit;
using Post = PostsTesting.UI_Models.Components.Post;

namespace PostsByMarko.FrontendTests.Tests
{
    [Collection("Frontend Collection")]
    public class PostTests : IAsyncLifetime
    {
        private readonly PostsByMarkoFactory postsByMarkoFactory;
        private IPage page;
        private HomePage homePage;
        private LoginPage loginPage;
        private readonly string testAdminEmail = TestingConstants.TEST_ADMIN_EMAIL;
        private readonly string testUserEmail = TestingConstants.TEST_USER_EMAIL;

        public PostTests(PostsByMarkoFactory postsByMarkoFactory)
        {
            this.postsByMarkoFactory = postsByMarkoFactory;
        }

        // Setup
        public async Task InitializeAsync()
        {
            page = await postsByMarkoFactory.browser.NewPageAsync();

            homePage = new HomePage(page);
            loginPage = new LoginPage(page);
        }

        // Teardown
        public async Task DisposeAsync()
        {
            if (page != null)
                await page.CloseAsync();
        }

        [Fact]
        public async Task should_create_post()
        {
            // Arrange
            await LoginWithUser(testUserEmail);

            var expectedTitle = new Faker().Commerce.Product();
            var expectedContent = new Faker().Commerce.ProductDescription();

            // Act
            await CreatePost(expectedTitle, expectedContent);

            var createdPost = new Post(page, homePage.postCard.Filter(new() { HasText = expectedTitle }).First);
            var title = await createdPost.title.TextContentAsync();
            var content = await createdPost.content.TextContentAsync();

            // Assert
            title.Should().Be(expectedTitle);
            content.Should().Be(expectedContent);
        }

        [Fact]
        public async Task should_update_a_post()
        {
            // Arrange
            await LoginWithUser(testAdminEmail);

            var post = new Post(page, homePage.postCard.First);

            var newTitle = new Faker().Commerce.Product();
            var newContent = new Faker().Commerce.ProductDescription();

            var oldPostTitle = await post.title.TextContentAsync();
            var oldPostContent = await post.content.TextContentAsync();

            // Act
            await post.ClickOnUpdateIcon();
            await homePage.modalComponent.FillInTitleInput(newTitle);
            await homePage.modalComponent.FillInContentInput(newContent);
            await homePage.modalComponent.updateButton.ClickAsync();
            await homePage.modalComponent.WaitForSuccessMessageToShowAndDisappear();

            post.Refresh();

            var updatedTitle = await post.title.TextContentAsync();
            var updatedContent = await post.content.TextContentAsync();

            // Assert
            updatedTitle.Should().NotBe(oldPostTitle);
            updatedTitle.Should().Be(newTitle);
            updatedContent.Should().NotBe(oldPostContent);
            updatedContent.Should().Be(newContent);
        }

        [Fact]
        public async Task should_delete_a_post()
        {
            // Arrange
            await LoginWithUser(testAdminEmail);

            var post = new Post(page, homePage.postCard.First);

            // Act
            await post.ClickOnDeleteIcon();
            await homePage.modalComponent.deleteButton.ClickAsync();
            await homePage.WaitForPostListSizeToChange();

            var postWithIdCount = await homePage.postCard.Locator($"#{post.Id}").CountAsync();

            // Assert
            postWithIdCount.Should().Be(0);
        }

        [Fact]
        public async Task should_hide_a_post()
        {
            // Arrange
            await LoginWithUser(testAdminEmail);

            var visiblePost = new Post(page, homePage.page.Locator(".post[data-hidden='false']").First);

            // Act
            await visiblePost.ClickOnHideIcon();
            await visiblePost.WaitForPostVisibilityToToggle();

            var hiddenState = await visiblePost.post.GetAttributeAsync("data-hidden");

            // Assert
            hiddenState.Should().Be("true");
            (await visiblePost.post.IsVisibleAsync()).Should().BeTrue();
            (await visiblePost.post.EvaluateAsync<double>("element => Number(getComputedStyle(element).opacity)")).Should().BeInRange(0.1, 0.99);
        }

        [Fact]
        public async Task should_view_post_details()
        {
            // Arrange
            await LoginWithUser(testAdminEmail);

            var post = new Post(page, homePage.postCard.First);
            var postTitle = await post.title.TextContentAsync();
            var postContent = await post.content.TextContentAsync();
            var postId = post.Id[5..];

            // Act
            await post.ClickOnPost();

            var detailsPage = new DetailsPage(page);

            await detailsPage.WaitForPage();

            var detailsTitle = await detailsPage.title.TextContentAsync();
            var detailsContent = await detailsPage.content.TextContentAsync();

            // Assert
            detailsPage.page.Url.Should().Contain(postId);
            detailsTitle.Should().Be(postTitle);
            detailsContent.Should().Be(postContent);
        }

        [Fact]
        public async Task should_edit_a_post()
        {
            // Arrange
            await LoginWithUser(testAdminEmail);

            var post = new Post(page, homePage.postCard.Last);
            var postContent = await post.content.TextContentAsync();

            await post.ClickOnPost();

            var detailsPage = new DetailsPage(page);
            var newContent = $"{new Faker().Commerce.ProductDescription()} with {new Faker().Commerce.Ean13()}";

            // Act
            await detailsPage.editButton.ClickAsync();
            await detailsPage.textArea.FillAsync(newContent);
            await detailsPage.saveButton.ClickAsync();
            await detailsPage.WaitForSuccessMessage();

            var successMessageText = await detailsPage.successMessage.TextContentAsync();
            var detailsContent = await detailsPage.content.TextContentAsync();

            // Assert
            successMessageText.Should().Be("Successfully updated Post!");
            detailsContent.Should().Be(newContent);

            await detailsPage.backButton.ClickAsync();
            post.Refresh();
            await Assertions.Expect(post.content).ToHaveTextAsync(newContent);

            var postCardContent = await post.content.TextContentAsync();

            postCardContent.Should().Be(newContent);
        }

        private async Task CreatePost(string title, string content)
        {
            await homePage.navComponent.dropdownMenu.ClickAsync();
            await homePage.navComponent.createPost.ClickAsync();
            await homePage.modalComponent.FillInTitleInput(title);
            await homePage.modalComponent.FillInContentInput(content);
            await homePage.modalComponent.createButton.ClickAsync();
            await homePage.WaitForPostListSizeToChange();
        }

        private async Task LoginWithUser(string email)
        {
            await loginPage.Visit();
            await loginPage.Login(email, TestingConstants.TEST_PASSWORD);
            await homePage.username.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        }
    }
}
