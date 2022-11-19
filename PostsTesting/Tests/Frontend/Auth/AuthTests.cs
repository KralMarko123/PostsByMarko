using PostsTesting.Tests.TestBase;
using Xunit;

namespace PostsTesting.Tests.Frontend.Auth
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
