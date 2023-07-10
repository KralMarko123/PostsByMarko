using Microsoft.Playwright;
using PostsByMarko.Shared.Constants;

namespace PostsTesting.UI_Models.Pages
{
    public class Page : Shared
    {
        protected static string baseUrl => $"{TestingConstants.DEV_CLIENT_ENDPOINT}";

        public Page(IPage page) : base(page) { }

        public ILocator title => page.Locator(".container__title");
        public ILocator description => page.Locator(".container__description");
        public ILocator footer => page.Locator(".container__footer");
        public ILocator back => page.Locator(".container__back");
        public ILocator errorMessage => page.Locator(".error");
        public ILocator errorSubmessage => page.Locator(".error__message");
    }
}
