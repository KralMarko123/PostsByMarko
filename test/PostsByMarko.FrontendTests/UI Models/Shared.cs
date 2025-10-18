using Microsoft.Playwright;

namespace PostsTesting.UI_Models
{
    public class Shared
    {
        public IPage page;

        public Shared(IPage page)
        {
            this.page = page;
        }

        protected ILocator button => page.Locator(".button");
        protected ILocator link => page.Locator(".link");
        protected ILocator errorMessage => page.Locator("p.error");
        protected ILocator successMessage => page.Locator("p.success");
    }
}
