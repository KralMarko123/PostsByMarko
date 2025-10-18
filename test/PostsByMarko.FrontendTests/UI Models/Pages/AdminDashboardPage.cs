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

        public async Task<List<string>> GetHeaders()
        {
            var headerTexts = new List<string>();
            var headers = await tableHeaders.AllAsync();

            foreach (var header in headers)
            {
                var text = await header.TextContentAsync();
                headerTexts.Add(text);
            }

            return headerTexts;
        }
    }
}
