using Microsoft.Playwright;
using PostsByMarko.Test.Shared.Constants;
using System.Text.RegularExpressions;

namespace PostsByMarko.FrontendTests.Helpers
{
    public static class PlaywrightHelpers
    {
        private static readonly int timeoutMs = TestingConstants.UI_TIMEOUT_IN_MILLISECONDS;

        public static async Task WaitForChildrenCountToChange(ILocator parentLocator)
        {
            var initialCount = await parentLocator.Locator(":scope > *").CountAsync();

            await Assertions.Expect(parentLocator.Locator(":scope > *")).Not.ToHaveCountAsync(initialCount, new() { Timeout = timeoutMs });
        }

        public static async Task WaitForTextToChange(ILocator locator)
        {
            var initialContent = await locator.InnerTextAsync();

            await Assertions.Expect(locator).Not.ToHaveTextAsync(initialContent, new() { Timeout = timeoutMs });
        }

        public static async Task WaitForClassToBePresent(ILocator locator, string className)
        {
            await Assertions.Expect(locator).ToHaveClassAsync(new Regex($@"\b{Regex.Escape(className)}\b"), new() { Timeout = timeoutMs });
        }

        public static async Task WaitForClassToBeRemoved(ILocator locator, string className)
        {
            await Assertions.Expect(locator).Not.ToHaveClassAsync(new Regex($@"\b{Regex.Escape(className)}\b"), new() { Timeout = timeoutMs });
        }

        public static async Task WaitForElementToVisible(ILocator locator)
        {
            await Assertions.Expect(locator).ToBeVisibleAsync(new() { Timeout = timeoutMs });
        }
    }
}
