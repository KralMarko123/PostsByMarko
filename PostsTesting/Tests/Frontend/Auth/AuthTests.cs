using PostsTesting.Tests.TestBase;
using Xunit;

namespace PostsTesting.Tests.Frontend.Auth
{
    public class AuthTests : UiTestBase
    {
        [Fact]
        public async Task VerifyUserCanRegister()
        {
            await VerifyUserCanBeRegistered();
        }

        [Fact]
        public async Task VerifyRegisterErrorMessages()
        {
            await VerifyErrorMessagesWhenRegistering();
        }
    }
}
