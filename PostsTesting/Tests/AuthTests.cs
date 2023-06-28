using PostsTesting.Tests.Frontend.Base;
using Xunit;

namespace PostsByMarko.FrontendTests.Frontend
{
    public class AuthTests : AuthUiTestBase
    {
        [Fact]
        public async Task VerifyUserCanLogin()
        {
            await VerifyUserCanBeLoggedIn();
        }

        [Fact]
        public async Task VerifyUserCanRegister()
        {
            await VerifyUserCanBeRegistered();
        }

        [Fact]
        public async Task VerifyLoginErrorMessages()
        {
            await VerifyErrorMessagesWhenLoggingIn();
        }

        [Fact]
        public async Task VerifyRegisterErrorMessages()
        {
            await VerifyErrorMessagesWhenRegistering();
        }
    }
}
