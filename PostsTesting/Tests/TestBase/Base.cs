using aspnetserver.Data.Models;
using Microsoft.Playwright;
using PostsTesting.Utility;
using PostsTesting.Utility.Constants;
using RestSharp;
using Xunit;

namespace PostsTesting.Tests
{
    public class Base : IAsyncLifetime
    {
        protected BrowserDriver driver;
        protected IBrowser browser;
        protected IPage page;
        protected User testUser = TestingConstants.TestUser;
        protected RestClient client;
        public async Task InitializeAsync()
        {
            driver = new BrowserDriver();
            browser = await driver.GetBrowserAsync();
            page = await browser.NewPageAsync();
            client = new RestClient(TestingConstants.ServerEndpoint);
        }

        public async Task DisposeAsync()
        {
            await driver.DestroyBrowser();
            await driver.DestroyPlaywright();
        }
    }
}
