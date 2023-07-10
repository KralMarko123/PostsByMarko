using Microsoft.Playwright;

namespace PostsTesting.UI_Models.Components
{
    public class Select : Component
    {
        public Select(IPage page) : base(page) { }

        private const string optionsListLocator = ".select__options";
        private const string optionLocator = ".select__option";
        public ILocator container => page.Locator(".select__container");
        public ILocator value => page.Locator("#selected__value");
        public ILocator clear => page.Locator(".select__clear");
        public ILocator arrow => page.Locator(".select__arrow");
        public ILocator optionsList => page.Locator(optionsListLocator);

        public async Task OpenSelect()
        {
            if (!await CheckIfSelectOptionsAreShown())
            {
                await container.ClickAsync();
                await optionsList.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
            }
        }

        public async Task CloseSelect()
        {
            if (await CheckIfSelectOptionsAreShown())
            {
                await container.ClickAsync();
                await optionsList.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Hidden });
            }
        }

        public async Task ClearSelect()
        {
            await clear.ClickAsync();
        }

        public async Task ClickOnOption(string optionText)
        {
            await GetOptionContainingText(optionText).ClickAsync();
        }

        public ILocator GetOptionContainingText(string text)
        {
            return page.Locator(optionLocator, new PageLocatorOptions { HasText = text });
        }

        public async Task<bool> CheckIfSelectOptionsAreShown()
        {
            return await page.Locator($"{optionsListLocator} show").IsVisibleAsync();
        }

    }
}
