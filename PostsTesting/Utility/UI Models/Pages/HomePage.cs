using Microsoft.Playwright;
using PostsTesting.Utility.Constants;
using Xunit;

namespace PostsTesting.Utility.Pages
{
    public class HomePage
    {
        private IPage page;
        private static string url => $"{AppConstants.UiEndpoint}/";
        public HomePage(IPage page) => this.page = page;


        public ILocator title => page.Locator(".container__title");
        public ILocator subtitle => page.Locator(".container__description");
        public ILocator postCard => page.Locator(".post");
        public ILocator postList => page.Locator(".posts__list");
        public ILocator createPostButton => page.Locator(".button");
        public ILocator infoMessage => page.Locator(".info__message");
        public Modal modal => new Modal(page);


        public async Task Visit()
        {
           await page.GotoAsync(url);
        }

        public async Task ClickCreatePostButton()
        {
            await createPostButton.ClickAsync();
        }

        public async Task CheckDefaultState()
        {
            var homeElementsAreDisplayed = await title.IsVisibleAsync() && await subtitle.IsVisibleAsync() && await createPostButton.IsVisibleAsync();
            var titleText = await title.TextContentAsync();
            Assert.True(homeElementsAreDisplayed);
            Assert.Equal("Welcome to our blog!", titleText);

            await ClickCreatePostButton();
            await modal.CheckVisibility("Create Form");
            await modal.CloseModal();

            var postsArePresent = await WaitForPostsToLoad();
            if (postsArePresent)
            {
                var postCount = await GetNumberOfPosts();
                for (int i = 0; i < postCount; i++)
                {
                    Post post = new Post(page, postCard.Nth(i));
                    await post.CheckPost();
                }
            }
            else
            {
                var infoMessageIsDisplyed = await infoMessage.IsVisibleAsync();
                var infoMessageText = await infoMessage.TextContentAsync();

                Assert.True(infoMessageIsDisplyed);
                Assert.Equal("Seems there are no posts.", infoMessageText);
            }
        }

        public async Task<bool> WaitForPostsToLoad()
        {
            await postList.WaitForAsync();
            bool postsAreVisible = await postList.IsVisibleAsync();
            return postsAreVisible;
        }

        public Post FindPostWithTitleAndContent(string titleToFindBy)
        {
            ILocator newlyCreatedPostCard = page.Locator(".post", new PageLocatorOptions { HasTextString = titleToFindBy });
            Post newlyCreatedPost = new Post(page, newlyCreatedPostCard);

            return newlyCreatedPost;
        }

        public async Task<int> GetNumberOfPosts()
        {
            var numberOfPosts = await postCard.CountAsync();
            return numberOfPosts;
        }
    }
}
