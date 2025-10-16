using Microsoft.Playwright;
using PostsByMarko.Test.Shared.Constants;

namespace PostsByMarko.FrontendTests.Drivers
{
    public class BrowserDriver
    {
        IPlaywright? playwrightInstance;
        IBrowser? browser;

        private async Task<IPlaywright> GetPlaywrightAsync()
        {
            playwrightInstance ??= await Playwright.CreateAsync();
            return playwrightInstance;
        }

        public async Task<IBrowser> GetChromiumBrowserAsync()
        {
            browser ??= await GetPlaywrightAsync()
                .Result
                .Chromium
                .LaunchAsync( new BrowserTypeLaunchOptions { Headless = TestingConstants.HEADLESS_BROWSER });

            return browser;
        }

        public async Task<IBrowser> GetFirefoxBrowserAsync()
        {
            browser ??= await GetPlaywrightAsync()
                .Result
                .Firefox
                .LaunchAsync(new BrowserTypeLaunchOptions { Headless = TestingConstants.HEADLESS_BROWSER });

            return browser;
        }

        public async Task DestroyPlaywrightAsync()
        {
            await Task.Run(() => playwrightInstance!.Dispose());
        }

    }
}

