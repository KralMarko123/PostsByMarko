using Microsoft.Playwright;
using PostsTesting.Utility.Constants;

namespace PostsTesting.Utility
{
    public class BrowserDriver
    {
        IPlaywright playwrightInstance;
        IBrowser browser;

        private async Task<IPlaywright> GetPlaywrightAsync()
        {
            playwrightInstance ??= await Playwright.CreateAsync();
            return playwrightInstance;
        }

        public async Task<IBrowser> GetBrowserAsync(bool headless = false)
        {
            switch (AppConstants.browserType)
            {
                case "Chrome":
                    browser ??= await GetPlaywrightAsync().Result.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                    {
                        Headless = headless
                    });
                    return browser;
                case "Firefox":
                    browser ??= await GetPlaywrightAsync().Result.Firefox.LaunchAsync(new BrowserTypeLaunchOptions
                    {
                        Headless = headless
                    });
                    return browser;
                default: return browser;
            }
        }

        public async Task DestroyPlaywright()
        {
            await Task.Run(() => playwrightInstance.Dispose());
        }

        public async Task DestroyBrowser()
        {
            await browser.CloseAsync();
        }

    }
}

