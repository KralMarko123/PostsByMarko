using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Playwright;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Shared.Constants;
using PostsTesting.Utility;
using Xunit;

namespace PostsByMarko.FrontendTests
{
    public class BaseFixture : WebApplicationFactory<IApiMarker>, IAsyncLifetime
    {
        public HttpClient client;

        public BrowserDriver driver;
        public IBrowser browser;
        public IPage page;
        public User adminUser = TestingConstants.TEST_USER;
        public User testUser = TestingConstants.TEST_USER;

        public async Task InitializeAsync()
        {
            client = CreateClient();
            
            driver = new BrowserDriver();
            browser = await driver.GetChromeBrowserAsync();
            page = await browser.NewPageAsync();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");
        }

        public async new Task DisposeAsync()
        {
            await base.DisposeAsync();
            await page.CloseAsync();
            await browser.CloseAsync();
            await browser.DisposeAsync();
            await driver.DestroyPlaywrightAsync();
        }
    }
}
