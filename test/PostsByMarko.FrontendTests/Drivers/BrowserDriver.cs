using Microsoft.Playwright;
using PostsByMarko.Shared.Constants;

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

        public async Task<IBrowser> GetChromeBrowserAsync()
        {
            browser ??= await GetPlaywrightAsync()
                .Result
                .Chromium
                .LaunchAsync( new BrowserTypeLaunchOptions { Headless = TestingConstants.HEADLESS_BROWSER });

            return browser;
        }

        public async Task DestroyPlaywrightAsync()
        {
            await Task.Run(() => playwrightInstance!.Dispose());
        }

    }
}

