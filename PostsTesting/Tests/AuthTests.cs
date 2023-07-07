using FluentAssertions;
using Microsoft.Playwright;
using PostsByMarko.Host.Data.Models;
using PostsTesting.Utility.UI_Models.Pages;
using Xunit;

namespace PostsByMarko.FrontendTests.Frontend
{
    [Collection("Frontend Collection")]
    public class AuthTests
    {
        private readonly User testUser;
        private readonly IPage page;
        private LoginPage loginPage => new LoginPage(page);

        public AuthTests(PostsByMarkoHostFactory postsByMarkoHostFactory)
        {
            page = postsByMarkoHostFactory.page;
            testUser = postsByMarkoHostFactory.testUser;
        }

        [Fact]
        public async Task Test()
        {
            await loginPage.Visit();
            await loginPage.Login(testUser.UserName, "some_password");
            await loginPage.errorMessage.WaitForAsync();

            var errorTitleText = await loginPage.errorMessage.TextContentAsync();
            errorTitleText.Should().Be("Invalid password for the given account");
        }
    }
}
