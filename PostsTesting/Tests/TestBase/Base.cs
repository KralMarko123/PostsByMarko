using Microsoft.Playwright;
using PostsTesting.Utility;
using PostsTesting.Utility.Constants;
using PostsTesting.Utility.Models;
using Xunit;

namespace PostsTesting.Tests
{
    public class Base : IAsyncLifetime
    {
        protected BrowserDriver driver;
        protected IBrowser browser;
        protected IPage page;
        protected User testUser = AppConstants.TestUser;
        public async Task InitializeAsync()
        {
            driver = new BrowserDriver();
            browser = await driver.GetBrowserAsync();
            page = await browser.NewPageAsync();
        }

        public async Task DisposeAsync()
        {
            await driver.DestroyBrowser();
            await driver.DestroyPlaywright();
        }
    }
}
