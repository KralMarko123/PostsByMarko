using aspnetserver.Data.Models;
using PostsTesting.Tests.TestBase;
using PostsTesting.Utility.Builders;
using Xunit;

namespace PostsTesting.Tests.Backend
{
    public class ApiTests : PostsApiTestBase
    {

        [Fact]
        public async Task VerifyPostsCanBeFetched()
        {
            var posts = await GetAllPosts();

            for (int i = 0; i < posts.Count; i++)
            {
                var post = posts[i];
                var postFetchedSeparately = await GetPostById(post.PostId.ToString());

                Assert.Multiple(() =>
                {
                    Assert.NotNull(postFetchedSeparately);
                    Assert.Equal(post.Title, postFetchedSeparately.Post.Title);
                    Assert.Equal(post.Content, postFetchedSeparately.Post.Content);
                    Assert.Equal(post.UserId, postFetchedSeparately.Post.UserId);
                });
            }
        }

        [Fact]
        public async Task VerifyPostCanBeCreated()
        {

       
        }
    }
}
