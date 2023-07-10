using Microsoft.Playwright;

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

        public async Task<IBrowser> GetChromeBrowserAsync(bool headless = false)
        {

            browser ??= await GetPlaywrightAsync()
                .Result
                .Chromium
                .LaunchAsync(new BrowserTypeLaunchOptions { Headless = headless });

            return browser;
        }

        public async Task<IBrowser> GetFirefoxBrowserAsync(bool headless = false)
        {

            browser ??= await GetPlaywrightAsync()
                .Result
                .Firefox
                .LaunchAsync(new BrowserTypeLaunchOptions { Headless = headless });

            return browser;
        }

        public async Task DestroyPlaywrightAsync()
        {
            await Task.Run(() => playwrightInstance!.Dispose());
        }

    }
}

