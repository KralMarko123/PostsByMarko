using Microsoft.Playwright;

namespace PostsTesting.Utility.UI_Models
{
    public class Shared
    {
        protected IPage page;

        public Shared(IPage page)
        {
            this.page = page;
        }

        protected ILocator button => page.Locator(".button");
        protected ILocator link => page.Locator(".link");


        public ILocator GetButtonWithText(string text)
        {
            return page.Locator(".button", new PageLocatorOptions { HasTextString = text });
        }
    }
}
