using Microsoft.Playwright;

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

        public async Task<IBrowser> GetChromeAsync(bool headless = false)
        {
            browser ??= await GetPlaywrightAsync().Result.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = headless
            });
            return browser;
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

