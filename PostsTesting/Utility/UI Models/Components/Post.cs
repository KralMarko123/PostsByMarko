using FluentAssertions;
using Microsoft.Playwright;
using PostsTesting.Utility.UI_Models.Components;

namespace PostsTesting.Utility.Pages
{
    public class Post : Component
    {
        private ILocator post;
        public readonly Modal modal;

        public Post(IPage page, ILocator post) : base(page)
        {
            this.post = post;
            modal = new Modal(page);
        }


        public ILocator title => post.Locator(".post__title");
        public ILocator content => post.Locator(".post__content");
        public ILocator idIcon => post.Locator(".post__id");
        public ILocator updateIcon => post.Locator(".post__update");
        public ILocator deleteIcon => post.Locator(".post__delete");


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
            await post.WaitForAsync();

            var postIsDisplayed = await title.IsVisibleAsync() && await content.IsVisibleAsync();
            postIsDisplayed.Should().BeTrue();

            await ClickOnUpdateIcon();
            await modal.CheckVisibility("Update Post");
            await modal.CloseModal();
            await ClickOnDeleteIcon();
            await modal.CheckVisibility("Delete Post");
            await modal.CloseModal();
        }

        public async Task CheckPostTitleAndContent(string expectedTitle, string expectedContent)
        {
            var postTitle = await title.TextContentAsync();
            var contentTitle = await content.TextContentAsync();

            postTitle.Should().Be(expectedTitle);
            contentTitle.Should().Be(expectedContent);
        }
    }
}
