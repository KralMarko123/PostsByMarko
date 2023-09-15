using FluentAssertions;
using Microsoft.Playwright;
using PostsByMarko.FrontendTests.Tests;
using PostsByMarko.Shared.Constants;
using PostsTesting.UI_Models.Pages;
using Xunit;

namespace PostsByMarko.FrontendTests.Frontend
{
    [Collection("Frontend Collection")]
    public class PostTests
    {
        private readonly IPage page;
        private HomePage homePage => new HomePage(page);
        private LoginPage loginPage => new LoginPage(page);

        public PostTests(PostsByMarkoFactory postsByMarkoHostFactory)
        {
            page = postsByMarkoHostFactory.page!;
        }

        [Fact]
        public async Task should_be_able_to_create_post()
        {
            if (!await homePage.home.IsVisibleAsync())
            {
                await loginPage.Visit();
                await loginPage.Login(TestingConstants.TEST_USER.Email, TestingConstants.TEST_PASSWORD);
            }

            await homePage.navComponent.dropdownMenu.HoverAsync();
            await homePage.navComponent.createPost.ClickAsync();
            await homePage.modalComponent.FillInTitleInput("Test title");
            await homePage.modalComponent.FillInContentInput("Test content");
            await homePage.modalComponent.createButton.ClickAsync();

            var newPostTitlte = await homePage.postCard.Last.Locator(".post-title").TextContentAsync();
            var newPostContent = await homePage.postCard.Last.Locator(".post-content").TextContentAsync();

            newPostTitlte.Should().Be("Test title");
            newPostContent.Should().Be("Test content");
        }
    }
}
