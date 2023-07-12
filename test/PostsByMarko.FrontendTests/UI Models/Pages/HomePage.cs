using FluentAssertions;
using Microsoft.Playwright;
using PostsTesting.UI_Models.Components;

namespace PostsTesting.UI_Models.Pages
{
    public class HomePage : Page
    {
        private static string url => $"{baseUrl}/";
        public readonly Modal modal;

        public HomePage(IPage page) : base(page)
        {
            modal = new Modal(page);
        }

        public ILocator home => page.Locator(".home");
        public ILocator username => page.Locator(".nav__username");
        public ILocator postCard => page.Locator(".post");
        public ILocator postList => page.Locator(".posts-list");
        public ILocator createPost => page.Locator(".action__item", new PageLocatorOptions { HasTextString = "Create Post" });
        public ILocator dropdownMenu => page.Locator(".nav__actions");

        public async Task Visit()
        {
            await page.GotoAsync(url);
        }

        public async Task HoverDropdownMenu()
        {
            await dropdownMenu.HoverAsync();
            await dropdownMenu.WaitForAsync();
        }

        public async Task ClickCreatePostButton()
        {
            await HoverDropdownMenu();
            await createPost.ClickAsync();
        }
    }
}
