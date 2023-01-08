using PostsTesting.Tests.Backend.Base;
using PostsTesting.Utility.Builders;
using PostsTesting.Utility.Extensions;
using System.Net;
using Xunit;
using FluentAssertions;
using aspnetserver.Data.Models;
using aspnetserver.Data.Models.Responses;
using Newtonsoft.Json;

namespace PostsTesting.Tests.Backend
{
    public class BackendTests : PostsApiTestBase
    {

        [Fact]
        public async Task VerifyPostsCanBeFetched()
        {
            var allPostsResponse = await GetAllPosts();
            var postsFromApi = JsonConvert.DeserializeObject<List<Post>>(allPostsResponse.Payload.ToString());
            var postsFromDb = await PostsDbTestBase.GetAllPosts();

            postsFromApi.Should().BeEquivalentTo(postsFromDb);
            allPostsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            for (int i = 0; i < postsFromApi.Count; i++)
            {
                var post = postsFromApi[i];
                var postByIdResult = await GetPostById(post.PostId.ToString());
                var postFromApi = JsonConvert.DeserializeObject<PostDetailsResponse>(postByIdResult.Payload.ToString());

                postByIdResult.StatusCode.Should().Be(HttpStatusCode.OK);
                postFromApi.Post.Should().BeEquivalentTo(post);
            }
        }

        [Fact]
        public async Task VerifyPostCanBeCreated()
        {
            var postsFromDb = await PostsDbTestBase.GetAllPosts();
            var postsCount = postsFromDb.Count();
            var data = new ObjectBuilder()
                .WithTitle($"Test Post")
                .WithContent($"Test Content")
                .Build();
            var createdPostResponse = await CreatePost(data);
            var newlyCreatedPost = JsonConvert.DeserializeObject<Post>(createdPostResponse.Payload.ToString());

            createdPostResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            createdPostResponse.Message.Should().Be("Post was created successfully");
            newlyCreatedPost.Should().NotBeNull();

            postsFromDb = await PostsDbTestBase.GetAllPosts();
            postsFromDb.Count.Should().Be(postsCount + 1);
            newlyCreatedPost.Title.Should().Be(data.GetProperty("Title").ToString());
            newlyCreatedPost.Content.Should().Be(data.GetProperty("Content").ToString());
        }

        [Fact]
        public async Task VerifyPostCanBeUpdated()
        {
            var updatedTitle = $"Updated Test Post";
            var updatedContent = $"Updated Test Content";
            var data = new ObjectBuilder()
                .WithTitle("Test Post")
                .WithContent("Test Content")
                .Build();
            var createdPostResponse = await CreatePost(data);
            var postToUpdate = JsonConvert.DeserializeObject<Post>(createdPostResponse.Payload.ToString());

            data = new ObjectBuilder()
                .WithPostId(postToUpdate.PostId.ToString())
                .WithTitle(updatedTitle)
                .WithContent(updatedContent)
                .Build();

            var updatedPostResponse = await UpdatePost(data);

            updatedPostResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            updatedPostResponse.Message.Should().Be("Post was updated successfully");


            var updatedPost = await PostsDbTestBase.GetPostById(postToUpdate.PostId);

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
            var createdPostResponse = await CreatePost(payload);
            var postToDelete = JsonConvert.DeserializeObject<Post>(createdPostResponse.Payload.ToString());
            var deletedPostResponse = await DeletePostById(postToDelete.PostId.ToString());

            deletedPostResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            deletedPostResponse.Message.Should().Be("Post was deleted successfully");

            var getPostByIdResponse = await GetPostByIdResponse(postToDelete.PostId.ToString());

            getPostByIdResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            getPostByIdResponse.Message.Should().Be($"Post with Id: {postToDelete.PostId} was not found");

            var getPostFromDb = async () => await PostsDbTestBase.GetPostById(postToDelete.PostId);
            await getPostFromDb.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task VerifyPostVisibilityCanBeToggled()
        {
            var payload = new ObjectBuilder()
                 .WithTitle("Test Post")
                 .WithContent("Test Content")
                 .Build();
            var createdPostResponse = await CreatePost(payload);
            var post = JsonConvert.DeserializeObject<Post>(createdPostResponse.Payload.ToString());

            post.IsHidden.Should().BeFalse();

            var postHiddenToggledResponse = await TogglePostVisibilityById(post.PostId.ToString());

            postHiddenToggledResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            postHiddenToggledResponse.Message.Should().Be("Post visibility was toggled successfully");

            var getPostByIdResponse = await GetPostById(post.PostId.ToString());
            var postDetails = JsonConvert.DeserializeObject<PostDetailsResponse>(getPostByIdResponse.Payload.ToString());

            postDetails.Post.IsHidden.Should().BeTrue();

            await TogglePostVisibilityById(post.PostId.ToString());

            getPostByIdResponse = await GetPostById(post.PostId.ToString());
            postDetails = JsonConvert.DeserializeObject<PostDetailsResponse>(getPostByIdResponse.Payload.ToString());

            postDetails.Post.IsHidden.Should().BeFalse();
        }

        [Fact]
        public async Task VerifyUserCanBeToggledForPost()
        {
            var payload = new ObjectBuilder()
                 .WithTitle("Test Post")
                 .WithContent("Test Content")
                 .Build();
            var createdPostResponse = await CreatePost(payload);
            var post = JsonConvert.DeserializeObject<Post>(createdPostResponse.Payload.ToString());

            post.AllowedUsers.Should().BeNullOrEmpty();

            var toggleResponse = await ToggleUserForPostById(post.PostId.ToString(), testUser.UserName);

            toggleResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            toggleResponse.Message.Should().Be("User was toggled successfully");

            var getPostByIdResponse = await GetPostById(post.PostId.ToString());
            var postDetails = JsonConvert.DeserializeObject<PostDetailsResponse>(getPostByIdResponse.Payload.ToString());

            postDetails.Post.AllowedUsers.Should().Contain(testUser.UserName);

            toggleResponse = await ToggleUserForPostById(post.PostId.ToString(), testUser.UserName);
            toggleResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            toggleResponse.Message.Should().Be("User was toggled successfully");

            getPostByIdResponse = await GetPostById(post.PostId.ToString());
            postDetails = JsonConvert.DeserializeObject<PostDetailsResponse>(getPostByIdResponse.Payload.ToString());

            postDetails.Post.AllowedUsers.Should().NotContain(testUser.UserName);
        }

    }
}
