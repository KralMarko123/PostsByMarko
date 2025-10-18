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
        private readonly User testUser = TestingConstants.TEST_USER;
        private readonly User testAdmin = TestingConstants.TEST_ADMIN;

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

            var dashboardTitle = await adminDashboardPage.containerTitle.TextContentAsync();
            var dashboardDescription = await adminDashboardPage.containerDescription.TextContentAsync();
            var userTableHeaders = await adminDashboardPage.tableHeaders.AllAsync();
            var userTableHeaderTexts = userTableHeaders.Select(th => th.TextContentAsync().Result);
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

            var testUserRow = new UserTableRow(page, testUser.Email);
            var userBadgeShown = await testUserRow.userBadge.IsVisibleAsync();
            var adminBadgeShown = await testUserRow.adminBadge.IsVisibleAsync();

            userBadgeShown.Should().Be(true);
            adminBadgeShown.Should().Be(false);

            await testUserRow.makeAdminButton.ClickAsync();
            await testUserRow.WaitForRowContentsToChange();

            adminBadgeShown = await testUserRow.adminBadge.IsVisibleAsync();
            adminBadgeShown.Should().Be(true);

            await testUserRow.removeAdminButton.ClickAsync();
            await testUserRow.WaitForRowContentsToChange();

            adminBadgeShown = await testUserRow.adminBadge.IsVisibleAsync();
            adminBadgeShown.Should().Be(false);
        }

        private async Task LoginWithUser(User user)
        {
            await loginPage.Visit();
            await loginPage.Login(user.Email, TestingConstants.TEST_PASSWORD);
            await homePage.username.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        }
    }
}
