using FluentAssertions;
using Microsoft.Playwright;
using PostsByMarko.FrontendTests.UI_Models.Components;
using PostsByMarko.FrontendTests.UI_Models.Pages;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Test.Shared.Constants;
using PostsTesting.UI_Models.Pages;
using Xunit;

namespace PostsByMarko.FrontendTests.Tests
{
    [Collection("Frontend Collection")]
    public class AdminTests : IAsyncLifetime
    {
        private readonly PostsByMarkoFactory postsByMarkoFactory;
        private IPage page;
        private AdminDashboardPage adminDashboardPage;
        private LoginPage loginPage;
        private HomePage homePage;
        private readonly User testAdmin = TestingConstants.TEST_ADMIN;
        private readonly User testUser = TestingConstants.TEST_USER;
        private readonly User marko = TestingConstants.MARKO;

        public AdminTests(PostsByMarkoFactory postsByMarkoFactory)
        {
            this.postsByMarkoFactory = postsByMarkoFactory;
        }

        // Setup
        public async Task InitializeAsync()
        {
            page = await postsByMarkoFactory.browser.NewPageAsync();

            homePage = new HomePage(page);
            loginPage = new LoginPage(page);
            adminDashboardPage = new AdminDashboardPage(page);
        }

        // Teardown
        public async Task DisposeAsync()
        {
            if (page != null) await page.CloseAsync();
        }

        [Fact]
        public async Task should_view_dashboard_data()
        {
            await LoginWithUser(testAdmin);
            await homePage.navComponent.dropdownMenu.HoverAsync();
            await homePage.navComponent.dashboard.ClickAsync();
            await adminDashboardPage.containerTitle.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

            var dashboardTitle = await adminDashboardPage.containerTitle.TextContentAsync();
            var dashboardDescription = await adminDashboardPage.containerDescription.TextContentAsync();
            var userTableHeaderTexts = await adminDashboardPage.GetHeaders();
            var chartsCount = await adminDashboardPage.charts.CountAsync();

            dashboardTitle.Should().Be("Admin Dashboard");
            dashboardDescription.Should().Be("Manage users and view statistics");
            userTableHeaderTexts.Should().Contain("User", "Number of Posts", "Last Posted", "Roles", "Actions");
            chartsCount.Should().Be(2);
        }

        [Fact]
        public async Task should_toggle_user_admin_privileges()
        {
            await LoginWithUser(testAdmin);
            await homePage.navComponent.dropdownMenu.HoverAsync();
            await homePage.navComponent.dashboard.ClickAsync();
            await adminDashboardPage.containerTitle.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

            var emailsNotToToggle = new List<string> { testAdmin.Email, marko.Email, testUser.Email };
            var emails = await adminDashboardPage.GetEmails();

            emails = [.. emails.Except(emailsNotToToggle)];

            var userRow = new UserTableRow(page, emails[new Random().Next(emails.Count)]);
            var adminBadgeShown = await userRow.adminBadge.IsVisibleAsync();

            adminBadgeShown.Should().Be(false);

            await userRow.makeAdminButton.ClickAsync();
            await userRow.WaitForSuccessMessageToShowAndDisappear();

            adminBadgeShown = await userRow.adminBadge.IsVisibleAsync();
            adminBadgeShown.Should().Be(true);

            await userRow.removeAdminButton.ClickAsync();
            await userRow.WaitForSuccessMessageToShowAndDisappear();

            adminBadgeShown = await userRow.adminBadge.IsVisibleAsync();
            adminBadgeShown.Should().Be(false);
        }

        [Fact]
        public async Task should_delete_a_user()
        {
            await LoginWithUser(testAdmin);
            await homePage.navComponent.dropdownMenu.HoverAsync();
            await homePage.navComponent.dashboard.ClickAsync();
            await adminDashboardPage.containerTitle.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

            var emailsNotToDelete = new List<string> { testAdmin.Email, marko.Email, testUser.Email };
            var emails = await adminDashboardPage.GetEmails();

            emails = [.. emails.Except(emailsNotToDelete)];

            var userRow = new UserTableRow(page, emails[new Random().Next(emails.Count)]);

            await userRow.deleteButton.ClickAsync();
            await userRow.WaitForSuccessMessageToShowAndDisappear();
            
            var isUserRowVisible = await userRow.userTableRow.IsVisibleAsync();

            isUserRowVisible.Should().Be(false);
        }

        private async Task LoginWithUser(User user)
        {
            await loginPage.Visit();
            await loginPage.Login(user.Email, TestingConstants.TEST_PASSWORD);
            await homePage.username.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        }
    }
}
