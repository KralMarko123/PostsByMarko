using Microsoft.Playwright;
using PostsTesting.UI_Models.Components;

namespace PostsByMarko.FrontendTests.UI_Models.Components
{
    public class Nav : Component
    {
        public Nav(IPage page) : base(page) { }

        public ILocator dropdownMenu => page.Locator(".nav__actions");
        public ILocator createPost => page.Locator(".action__item", new PageLocatorOptions { HasTextString = "Create Post" });


    }
}
