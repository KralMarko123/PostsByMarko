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


        public async Task CheckPost()
        {
            bool postIsDisplayed = await title.IsVisibleAsync() && await content.IsVisibleAsync();
            Assert.True(postIsDisplayed);

            await updateIcon.ClickAsync();
            await modal.CheckVisibility("Update Form");
            await modal.CloseModal();
            await deleteIcon.ClickAsync();
            await modal.CheckVisibility("Delete Post");
            await modal.CloseModal();
        }

        public async Task ClickOnPost()
        {
            await post.ClickAsync();
        }
    }
}
