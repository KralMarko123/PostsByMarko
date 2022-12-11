using aspnetserver.Data.Models;
using PostsTesting.Utility.Constants;
using RestSharp;
using Xunit;

namespace PostsTesting.Tests
{
    public class BackendBase : IAsyncLifetime
    {
        protected RestClient client;
        protected User testUser = TestingConstants.TestUser;

        public async Task InitializeAsync()
        {
            client = new RestClient(TestingConstants.ServerEndpoint);
        }

        public async Task DisposeAsync()
        {
            client.Dispose();
        }
    }
}
