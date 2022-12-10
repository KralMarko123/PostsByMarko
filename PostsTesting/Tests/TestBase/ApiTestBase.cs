using aspnetserver.Helper;
using Xunit;

namespace PostsTesting.Tests.TestBase
{
    public class ApiTestBase : Base, IAsyncLifetime
    {
        private readonly IJwtHelper jwtHelper;

        public ApiTestBase(IJwtHelper jwtHelper)
        {
            this.jwtHelper = jwtHelper;
        }

        public async Task InitializeAsync() => await base.InitializeAsync();

        public async Task AddAuthenticationTokenToClient()
        {
            var token = jwtHelper.CreateTokenAsync(testUser);
        }

    }
}
