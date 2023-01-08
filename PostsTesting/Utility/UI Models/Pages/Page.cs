using Microsoft.Playwright;
using PostsTesting.Utility.Constants;

namespace PostsTesting.Utility.UI_Models.Pages
{
    public class Page : Shared
    {
        protected static string baseUrl => $"{TestingConstants.UiEndpoint}";

        public Page(IPage page) : base(page) { }

        protected ILocator title => page.Locator(".container__title");
        protected ILocator description => page.Locator(".container__description");
        protected ILocator footer => page.Locator(".container__footer");
        public ILocator back => page.Locator(".container__back");
        protected ILocator errorMessage => page.Locator(".error");
        protected ILocator errorSubmessage => page.Locator(".error__message");
    }
}
