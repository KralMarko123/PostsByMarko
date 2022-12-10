using aspnetserver.Helper;
using PostsTesting.Tests.TestBase;
using Xunit;

namespace PostsTesting.Tests.Backend
{
    public class ApiTests : PostsApiTestBase
    {
      
        [Fact]
        public async Task VerifyPostsCanBeFetched()
        {
            var posts = await GetAllPosts();
            Assert.NotNull(posts);
        }
    }
}
