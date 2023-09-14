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

        public async Task<IBrowser> GetChromeBrowserAsync()
        {
            browser ??= await GetPlaywrightAsync()
                .Result
                .Chromium
                .LaunchAsync();

            return browser;
        }

        public async Task DestroyPlaywrightAsync()
        {
            await Task.Run(() => playwrightInstance!.Dispose());
        }

    }
}

