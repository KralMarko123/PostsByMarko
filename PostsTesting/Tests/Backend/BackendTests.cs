using PostsTesting.Tests.Backend.Base;
using PostsTesting.Utility.Builders;
using PostsTesting.Utility.Extensions;
using System.Net;
using Xunit;
using FluentAssertions;

namespace PostsTesting.Tests.Backend
{
    public class BackendTests : PostsApiTestBase
    {

        [Fact]
        public async Task VerifyPostsCanBeFetched()
        {
            var posts = await GetAllPosts();
            var postsFromDb = await PostsDbTestBase.GetAllPosts();

            posts.Should().BeEquivalentTo(postsFromDb);

            for (int i = 0; i < posts.Count; i++)
            {
                var post = posts[i];
                var postFetchedSeparately = await GetPostById(post.PostId.ToString());

                postFetchedSeparately.Should().NotBeNull();
                postFetchedSeparately.Post.Should().BeEquivalentTo(post);
            }
        }

        [Fact]
        public async Task VerifyPostCanBeCreated()
        {
            var postsFromDb = await PostsDbTestBase.GetAllPosts();
            var postsCount = postsFromDb.Count();
            var payload = new ObjectBuilder()
                .WithTitle($"Test Post")
                .WithContent($"Test Content")
                .Build();
            var createdPostResponse = await CreatePost(payload);

            createdPostResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            createdPostResponse.Content.Should().Contain("Post was created successfully");

            postsFromDb = await PostsDbTestBase.GetAllPosts();
            postsFromDb.Count.Should().Be(postsCount + 1);
            postsFromDb.Last().Title.Should().Be(payload.GetProperty("Title").ToString());
            postsFromDb.Last().Content.Should().Be(payload.GetProperty("Content").ToString());
        }

        [Fact]
        public async Task VerifyPostCanBeUpdated()
        {
            var updatedTitle = $"Updated Test Post";
            var updatedContent = $"Updated Test Content";
            var payload = new ObjectBuilder()
                .WithTitle("Test Post")
                .WithContent("Test Content")
                .Build();

            await CreatePost(payload);

            var allPosts = await GetAllPosts();
            var postToUpdateId = allPosts.Last().PostId;

            payload = new ObjectBuilder()
                .WithPostId(postToUpdateId.ToString())
                .WithTitle(updatedTitle)
                .WithContent(updatedContent)
                .Build();

            var updatedPostResponse = await UpdatePost(payload);
            updatedPostResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            updatedPostResponse.Content.Should().Contain("Post was updated successfully");


            var updatedPost = await PostsDbTestBase.GetPostById(postToUpdateId);
            updatedPost.Title.Should().Be(updatedTitle);
            updatedPost.Content.Should().Be(updatedContent);
            updatedPost.LastUpdatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task VerifyPostCanBeDeleted()
        {
            var payload = new ObjectBuilder()
                 .WithTitle("Test Post")
                 .WithContent("Test Content")
                 .Build();

            await CreatePost(payload);

            var postsFromDb = await PostsDbTestBase.GetAllPosts();
            var postToDeleteId = postsFromDb.Last().PostId;

            var deletedPostResponse = await DeletePostById(postToDeleteId.ToString());
            deletedPostResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            deletedPostResponse.Content.Should().Contain("Post was deleted successfully");

            var findPostByIdResponse = await GetPostByIdResponse(postToDeleteId.ToString());
            findPostByIdResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            findPostByIdResponse.Content.Should().Contain($"Post with Id: {postToDeleteId} was not found");

            var getPostFromDb = async () => await PostsDbTestBase.GetPostById(postToDeleteId);
            await getPostFromDb.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task VerifyPostVisibilityCanBeToggled()
        {
            var payload = new ObjectBuilder()
                 .WithTitle("Test Post")
                 .WithContent("Test Content")
                 .Build();

            await CreatePost(payload);

            var postsFromDb = await PostsDbTestBase.GetAllPosts();

            var postId = postsFromDb.Last().PostId.ToString();
            var post = await GetPostById(postId);
            post.Post.IsHidden.Should().BeFalse();

            var postHiddenToggledResponse = await TogglePostVisibilityById(postId);
            postHiddenToggledResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            postHiddenToggledResponse.Content.Should().Contain("Post visibility was toggled successfully");

            post = await GetPostById(postId);
            post.Post.IsHidden.Should().BeTrue();

            await TogglePostVisibilityById(postId);
            post = await GetPostById(postId);
            post.Post.IsHidden.Should().BeFalse();
        }

    }
}
