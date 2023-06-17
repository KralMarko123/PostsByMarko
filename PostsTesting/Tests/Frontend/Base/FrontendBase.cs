using Microsoft.Playwright;
using PostsByMarko.Host.Data.Models;
using PostsTesting.Utility;
using PostsTesting.Utility.Constants;
using Xunit;

namespace PostsTesting.Tests.Frontend.Base
{
    public class FrontendBase : IAsyncLifetime
    {
        protected BrowserDriver? driver;
        protected IBrowser? browser;
        protected IPage? page;
        protected User adminUser = TestingConstants.AdminUser;
        protected User testUser = TestingConstants.TestUser;
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
