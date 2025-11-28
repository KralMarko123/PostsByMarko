//using Bogus;
//using FluentAssertions;
//using Microsoft.Playwright;
//using PostsByMarko.Host.Data.Entities;
//using PostsByMarko.Test.Shared.Constants;
//using PostsTesting.UI_Models.Pages;
//using Xunit;
//using Post = PostsTesting.UI_Models.Components.Post;

//namespace PostsByMarko.FrontendTests.Tests
//{
//    [Collection("Frontend Collection")]
//    public class PostTests : IAsyncLifetime
//    {
//        private readonly PostsByMarkoFactory postsByMarkoFactory;
//        private IPage page;
//        private HomePage homePage;
//        private LoginPage loginPage;
//        private readonly User testUser = TestingConstants.TEST_USER;
//        private readonly User testAdmin = TestingConstants.TEST_ADMIN;

//        public PostTests(PostsByMarkoFactory postsByMarkoFactory)
//        {
//            this.postsByMarkoFactory = postsByMarkoFactory;
//        }

//        // Setup
//        public async Task InitializeAsync()
//        {
//            page = await postsByMarkoFactory.browser.NewPageAsync();

//            homePage = new HomePage(page);
//            loginPage = new LoginPage(page);
//        }

//        // Teardown
//        public async Task DisposeAsync()
//        {
//            if(page != null) await page.CloseAsync();
//        }

//        [Fact]
//        public async Task should_create_post()
//        {
//            await LoginWithUser(testUser);

//            var expectedTitle = new Faker().Commerce.Product();
//            var expectedContent = new Faker().Commerce.ProductDescription();

//            await CreatePost(expectedTitle, expectedContent);

//            var createdPost = new Post(page, homePage.postCard.First);
//            var title = await createdPost.title.TextContentAsync();
//            var content = await createdPost.content.TextContentAsync();

//            title.Should().Be(expectedTitle);
//            content.Should().Be(expectedContent);
//        }

//        [Fact]
//        public async Task should_update_a_post()
//        {
//            await LoginWithUser(testAdmin);

//            var post = new Post(page, homePage.postCard.First);

//            var newTitle = new Faker().Commerce.Product();
//            var newContent = new Faker().Commerce.ProductDescription();

//            var oldPostTitle = await post.title.TextContentAsync();
//            var oldPostContent = await post.content.TextContentAsync();

//            await post.ClickOnUpdateIcon();
//            await homePage.modalComponent.FillInTitleInput(newTitle);
//            await homePage.modalComponent.FillInContentInput(newContent);
//            await homePage.modalComponent.updateButton.ClickAsync();
//            await homePage.modalComponent.WaitForSuccessMessageToShowAndDisappear();
            
//            post.Refresh();

//            var updatedTitle = await post.title.TextContentAsync();
//            var updatedContent = await post.content.TextContentAsync();

//            updatedTitle.Should().NotBe(oldPostTitle);
//            updatedTitle.Should().Be(newTitle);
//            updatedContent.Should().NotBe(oldPostContent);
//            updatedContent.Should().Be(newContent);
//        }

//        [Fact]
//        public async Task should_delete_a_post()
//        {
//            await LoginWithUser(testAdmin);

//            var post = new Post(page, homePage.postCard.First);

//            await post.ClickOnDeleteIcon();
//            await homePage.modalComponent.deleteButton.ClickAsync();
//            await homePage.WaitForPostListSizeToChange();

//            var postWithIdCount = await homePage.postCard.Locator($"#{post.Id}").CountAsync();

//            postWithIdCount.Should().Be(0);
//        }

//        [Fact]
//        public async Task should_hide_a_post()
//        {
//            await LoginWithUser(testAdmin);

//            var visiblePost = new Post(page, homePage.page.Locator(".post:not(.hidden)").First);

//            await visiblePost.ClickOnHideIcon();
//            await visiblePost.WaitForPostVisibilityToToggle();

//            var postClassnames = await visiblePost.post.GetAttributeAsync("class");
            
//            postClassnames.Should().Contain("hidden");
//        }

//        [Fact]
//        public async Task should_view_created_post()
//        {
//            await LoginWithUser(testAdmin);

//            var expectedTitle = new Faker().Commerce.Product();
//            var expectedContent = new Faker().Commerce.ProductDescription();

//            await CreatePost(expectedTitle, expectedContent);

//            var createdPost = new Post(page, homePage.postCard.First);
//            var postId = createdPost.Id[5..];

//            await createdPost.ClickOnPost();
            
//            var detailsPage = new DetailsPage(page);

//            await detailsPage.WaitForPage();

//            var detailsTitle = await detailsPage.title.TextContentAsync();
//            var detailsAuthor = await detailsPage.author.TextContentAsync();
//            var detailsDate = await detailsPage.date.TextContentAsync();
//            var detailsContent = await detailsPage.content.TextContentAsync();

//            detailsPage.page.Url.Should().Contain(postId);
//            detailsTitle.Should().Be(expectedTitle);
//            detailsContent.Should().Be(expectedContent);
//            detailsAuthor.Should().Be($"By {testAdmin.FirstName} {testAdmin.LastName}");
//            detailsDate.Should().Be(DateTime.Today.ToString("d MMMM yyyy"));
//        }

//        [Fact]
//        public async Task should_edit_a_post()
//        {
//            await LoginWithUser(testAdmin);

//            var post = new Post(page, homePage.postCard.Last);
//            var postContent = await post.content.TextContentAsync();
//            var postId = post.Id[5..];

//            await post.ClickOnPost();

//            var detailsPage = new DetailsPage(page);
//            var newContent = $"{new Faker().Commerce.ProductDescription} with {new Faker().Commerce.Ean13()}";

//            await detailsPage.editButton.ClickAsync();
//            await detailsPage.textArea.FillAsync(newContent);
//            await detailsPage.saveButton.ClickAsync();
//            await detailsPage.WaitForSuccessMessage();

//            var successMessageText = await detailsPage.successMessage.TextContentAsync();
//            var detailsContent = await detailsPage.content.TextContentAsync();

//            successMessageText.Should().Be("Post was updated successfully");
//            detailsContent.Should().Be(newContent);
            
//            await detailsPage.backButton.ClickAsync();
//            post.Refresh();

//            var postCardContent = await post.content.TextContentAsync();

//            postCardContent.Should().Be(newContent);
//        }

//        private async Task CreatePost(string title, string content)
//        {
//            await homePage.navComponent.dropdownMenu.HoverAsync();
//            await homePage.navComponent.createPost.ClickAsync();
//            await homePage.modalComponent.FillInTitleInput(title);
//            await homePage.modalComponent.FillInContentInput(content);
//            await homePage.modalComponent.createButton.ClickAsync();
//            await homePage.WaitForPostListSizeToChange();
//        }

//        private async Task LoginWithUser(User user)
//        {
//            await loginPage.Visit();
//            await loginPage.Login(user.Email, TestingConstants.TEST_PASSWORD);
//            await homePage.username.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
//        }
//    }
//}
