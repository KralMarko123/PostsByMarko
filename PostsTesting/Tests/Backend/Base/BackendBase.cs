using aspnetserver.Data.Models;
using PostsTesting.Tests.Backend.Base;
using PostsTesting.Utility.Constants;
using RestSharp;
using Xunit;

namespace PostsTesting.Tests
{
    public class BackendBase : TestingFactory, IAsyncLifetime
    {
        protected RestClient client;
        protected User testUser = TestingConstants.TestUser;

        public new async Task InitializeAsync()
        {
            await base.InitializeAsync();

            client = new RestClient(TestingConstants.serverEndpoint);
        }

        public new async Task DisposeAsync()
        {
            client.Dispose();
            await PostsDbTestBase.DeleteAllTestPosts();
        }
    }
}
