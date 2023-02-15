using aspnetserver.Data.Models;
using Microsoft.Playwright;
using PostsTesting.Utility;
using PostsTesting.Utility.Constants;
using Xunit;

namespace PostsTesting.Tests.Frontend.Base
{
    public class FrontendBase : IAsyncLifetime
    {
        protected BrowserDriver driver;
        protected IBrowser browser;
        protected IPage page;
        protected User adminUser = TestingConstants.AdminUser;
        protected User testUser = TestingConstants.TestUser;
        public new async Task InitializeAsync()
        {
            driver = new BrowserDriver();
            browser = await driver.GetBrowserAsync();
            page = await browser.NewPageAsync();
        }

        public new async Task DisposeAsync()
        {
            await driver.DestroyBrowser();
            await driver.DestroyPlaywright();
        }
    }
}
