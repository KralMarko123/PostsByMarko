using Microsoft.Playwright;
using PostsTesting.UI_Models.Pages;

namespace PostsByMarko.FrontendTests.UI_Models.Pages
{
    internal class AdminDashboardPage : Page
    {
        private static string url => $"{baseUrl}/admin";

        public AdminDashboardPage(IPage page) : base(page) { }
        
        public ILocator tableHeaders => page.Locator("table thead th");
        public ILocator tableRows => page.Locator("table tbody tr");
        public ILocator charts => page.Locator(".chart");

        public async Task Visit()
        {
            await page.GotoAsync(url);
        }
    }
}
