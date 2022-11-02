using Microsoft.Playwright;
using PostsTesting.Utility;
using Xunit;

namespace PostsTesting.Tests
{
    public class Base : IAsyncLifetime
    {
        protected BrowserDriver driver;
        protected IBrowser browser;
        protected IPage page;

        public async Task InitializeAsync()
        {
            driver = new BrowserDriver();
            browser = await driver.GetChromeAsync();
            page = await browser.NewPageAsync();
        }

        public async Task DisposeAsync()
        {
            await driver.DestroyBrowser();
            await driver.DestroyPlaywright();
        }
    }
}
