using Microsoft.Playwright;
using Xunit;

namespace PostsTesting.Utility.Pages
{
    public class Post
    {
        private IPage page;
        private ILocator post;
        public Post(IPage page, ILocator post)
        {
            this.page = page;
            this.post = post;
        }


        public ILocator title => post.Locator(".post__title");
        public ILocator content => post.Locator(".post__content");
        public ILocator idIcon => post.Locator(".post__id");
        public ILocator updateIcon => post.Locator(".post__update");
        public ILocator deleteIcon => post.Locator(".post__delete");
        public Modal modal => new Modal(page);


        public async Task ClickOnPost()
        {
            await post.ClickAsync();
        }

        public async Task ClickOnUpdateIcon()
        {
            await updateIcon.ClickAsync();
        }

        public async Task ClickOnDeleteIcon()
        {
            await deleteIcon.ClickAsync();
        }

        public async Task CheckPost()
        {
            bool postIsDisplayed = await title.IsVisibleAsync() && await content.IsVisibleAsync();
            Assert.True(postIsDisplayed);

            await ClickOnUpdateIcon();
            await modal.CheckVisibility("Update Form");
            await modal.CloseModal();
            await ClickOnDeleteIcon();
            await modal.CheckVisibility("Delete Post");
            await modal.CloseModal();
        }

        public async Task CheckPostTitleAndContent(string expectedTitle, string expectedContent)
        {
            var postTitle = await title.TextContentAsync();
            var contentTitle = await content.TextContentAsync();

            Assert.Equal(expectedTitle, postTitle);
            Assert.Equal(expectedContent, contentTitle);
        }
    }
}
