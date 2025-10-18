using Microsoft.Playwright;
using PostsByMarko.FrontendTests.Helpers;
using PostsTesting.UI_Models.Components;

namespace PostsByMarko.FrontendTests.UI_Models.Components
{
    public class UserTableRow : Component
    {
        public ILocator userTableRow;
        public readonly string user;

        public UserTableRow(IPage page, string user) : base(page)
        {
            userTableRow = page.Locator($"[data-email='{user}']");
            this.user = user;
        }

        public ILocator userBadge => userTableRow.Locator(".table-badge", new LocatorLocatorOptions() { HasText = "User" });
        public ILocator adminBadge => userTableRow.Locator(".table-badge", new LocatorLocatorOptions() { HasText = "Admin" });
        public ILocator deleteButton => userTableRow.Locator(".table-button.error");
        public ILocator removeAdminButton => userTableRow.Locator(".table-button.warning");
        public ILocator makeAdminButton => userTableRow.Locator(".table-button.success");

        public void Refresh()
        {
            userTableRow = page.Locator($"[data-email='{user}']");
        }

        public async Task WaitForRowContentsToChange()
        {
            await PlaywrightHelpers.WaitForInnerHtmlToChange(userTableRow);
            Refresh();
        }
    }
}
