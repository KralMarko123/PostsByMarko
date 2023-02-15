using aspnetserver.Data.Models;
using PostsTesting.Tests.Backend.Base;
using PostsTesting.Utility.Constants;
using RestSharp;
using Xunit;

namespace PostsTesting.Tests
{
    public class BackendBase : IAsyncLifetime
    {
        protected RestClient? client;
        protected User testUser = TestingConstants.TestUser;

        public async Task InitializeAsync()
        {
            await Task.Run(() => client = new RestClient(TestingConstants.serverEndpoint));
        }

        public async Task DisposeAsync()
        {
            client?.Dispose();
            await PostsDbTestBase.DeleteAllTestPosts();
        }
    }
}
