using Microsoft.Playwright;
using PostsByMarko.Shared.Constants;

namespace PostsTesting.UI_Models.Pages
{
    public class Page : Shared
    {
        protected static string baseUrl => $"{TestingConstants.DEV_CLIENT_ENDPOINT}";

        public Page(IPage page) : base(page) { }

        public ILocator containerTitle => page.Locator(".container-title");
        public ILocator containerDescription => page.Locator(".container-description");
        public ILocator error => page.Locator(".error");
    }
}
