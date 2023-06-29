using Microsoft.Playwright;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Shared.Constants;
using PostsTesting.Utility;
using Xunit;

namespace PostsTesting.Tests.Frontend.Base
{
    public class FrontendBase : IAsyncLifetime
    {
        protected BrowserDriver driver;
        protected IBrowser browser;
        protected IPage page;
        protected User adminUser = TestingConstants.TEST_USER;
        protected User testUser = TestingConstants.TEST_USER;

        public async Task InitializeAsync()
        {
            driver = new BrowserDriver();
            browser = await driver.GetChromeBrowserAsync();
            page = await browser.NewPageAsync();
        }

        public async Task DisposeAsync()
        {
            await driver.DestroyPlaywrightAsync();
        }
    }
}
