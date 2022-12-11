using PostsTesting.Tests.Backend.Base;
using PostsTesting.Utility;
using PostsTesting.Utility.Builders;
using PostsTesting.Utility.Extensions;
using System.Net;
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
            var postToCreate = new ObjectBuilder()
                .WithTitle($"New Post No. {RandomDataGenerator.GetRandomNumberWithMax(100)}")
                .WithContent($"New Content No. {RandomDataGenerator.GetRandomNumberWithMax(100)}")
                .Build();
            var allPosts = await GetAllPosts();
            var allPostsCount = allPosts.Count;
            var createdPostResponse = await CreatePost(postToCreate);

            Assert.Equal(HttpStatusCode.OK, createdPostResponse.StatusCode);
            Assert.Contains("Post was created successfully.", createdPostResponse.Content);

            allPosts = await GetAllPosts();
            Assert.Multiple(() =>
            {
                Assert.True(allPosts.Count == allPostsCount + 1);
                Assert.Equal(postToCreate.GetProperty("Title"), allPosts.Last().Title);
                Assert.Equal(postToCreate.GetProperty("Content"), allPosts.Last().Content);
            });
        }

        [Fact]
        public async Task VerifyPostCanBeUpdated()
        {
            var updatedTitle = $"Updated Post No. {RandomDataGenerator.GetRandomNumberWithMax(100)}";
            var updatedContent = $"Updated Content No. {RandomDataGenerator.GetRandomNumberWithMax(100)}";

            var payload = new ObjectBuilder()
                .WithTitle("New Post")
                .WithContent("New Content")
                .Build();

            await CreatePost(payload);

            var allPosts = await GetAllPosts();
            var postToUpdateId = allPosts.Last().PostId.ToString();

            payload = new ObjectBuilder()
                .WithPostId(postToUpdateId)
                .WithTitle(updatedTitle)
                .WithContent(updatedContent)
                .Build();

            var updatedPostResponse = await UpdatePost(payload);
            Assert.Equal(HttpStatusCode.OK, updatedPostResponse.StatusCode);
            Assert.Contains("Post was updated successfully.", updatedPostResponse.Content);

            var updatedPost = await GetPostById(postToUpdateId);
            Assert.Equal(updatedTitle, updatedPost.Post.Title);
            Assert.Equal(updatedContent, updatedPost.Post.Content);
        }

        [Fact]
        public async Task VerifyPostCanBeDeleted()
        {
            var payload = new ObjectBuilder()
                 .WithTitle("New Post")
                 .WithContent("New Content")
                 .Build();

            await CreatePost(payload);

            var allPosts = await GetAllPosts();
            var postToDeleteId = allPosts.Last().PostId.ToString();

            var deletedPostResponse = await DeletePostById(postToDeleteId);
            Assert.Equal(HttpStatusCode.OK, deletedPostResponse.StatusCode);
            Assert.Contains("Post was deleted successfully.", deletedPostResponse.Content);

            var findPostByIdResponse = await GetPostByIdResponse(postToDeleteId);
            Assert.Equal(HttpStatusCode.NotFound, findPostByIdResponse.StatusCode);
            Assert.Contains($"Post with id: {postToDeleteId} was not found.", findPostByIdResponse.Content);

            allPosts = await GetAllPosts();
            Assert.Null(allPosts.Find(p => p.PostId.ToString() == postToDeleteId));
        }
    }
}
