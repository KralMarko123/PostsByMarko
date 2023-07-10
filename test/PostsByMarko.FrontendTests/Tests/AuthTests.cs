using FluentAssertions;
using Microsoft.Playwright;
using PostsByMarko.FrontendTests.Tests;
using PostsTesting.UI_Models.Pages;
using Xunit;

namespace PostsByMarko.FrontendTests.Frontend
{
    [Collection("Frontend Collection")]
    public class AuthTests
    {
        private readonly IPage page;
        private LoginPage loginPage => new LoginPage(page);

        public AuthTests(PostsByMarkoHostFactory postsByMarkoHostFactory)
        {
            page = postsByMarkoHostFactory.page;
        }

        [Fact]
        public async Task Test()
        {
            await loginPage.Visit();
            await loginPage.Login("test_user@test.com", "some_password");
            await loginPage.errorMessage.WaitForAsync();

            var errorTitleText = await loginPage.errorMessage.TextContentAsync();
            errorTitleText.Should().Be("Invalid password for the given account");
        }
    }
}
