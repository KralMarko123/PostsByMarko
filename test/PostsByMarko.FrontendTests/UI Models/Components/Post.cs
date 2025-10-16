using Microsoft.Playwright;
using PostsByMarko.FrontendTests.Helpers;

namespace PostsTesting.UI_Models.Components
{
    public class Post : Component
    {
        public ILocator post;
        public readonly Modal modal;

        public Post(IPage page, ILocator post) : base(page)
        {
            this.post = post;
            modal = new Modal(page);
        }

        public string Id => post.GetAttributeAsync("id").Result!;
        public ILocator title => post.Locator(".post-title");
        public ILocator content => post.Locator(".post-content");
        public ILocator updateIcon => post.Locator(".post-icon.update");
        public ILocator deleteIcon => post.Locator(".post-icon.delete");
        public ILocator hideIcon => post.Locator(".post-icon.hide");

        public void Refresh()
        {
            post = page.Locator($"#{Id}");
        }

        public async Task WaitForPostContentsToChange()
        {
            await PlaywrightHelpers.WaitForTextToChange(post);
            Refresh();
        }

        public async Task WaitForPostVisibilityToToggle(bool isHidden = false)
        {
            Refresh();

            if (isHidden) await PlaywrightHelpers.WaitForClassToBeRemoved(post, "hidden");
            else await PlaywrightHelpers.WaitForClassToBePresent(post, "hidden");
        }

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

        public async Task ClickOnHideIcon()
        {
            await hideIcon.ClickAsync();
        }
    }
}
