using FluentAssertions;
using Microsoft.AspNetCore.SignalR.Client;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Requests;
using PostsByMarko.Host.Data.Repositories.Posts;
using PostsByMarko.Test.Shared.Constants;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PostsByMarko.IntegrationTests.Hubs
{
    [Collection("IntegrationCollection")]
    public class PostHubTests : IAsyncLifetime
    {
        private readonly PostsByMarkoApiFactory postsByMarkoApiFactory;
        private readonly HttpClient client;
        private HubConnection? hubConnection;
        private readonly string controllerPrefix = "/api/post";

        public PostHubTests(PostsByMarkoApiFactory postsByMarkoApiFactory)
        {
            this.postsByMarkoApiFactory = postsByMarkoApiFactory;

            client = postsByMarkoApiFactory.client!;
        }

        public async Task InitializeAsync()
        {
            await postsByMarkoApiFactory.RecreateAndSeedDatabaseAsync();
            await postsByMarkoApiFactory.AuthenticateClientAsync(client);

            hubConnection = postsByMarkoApiFactory.CreateHubConnection("postHub");

            await hubConnection.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await hubConnection!.StopAsync();
            await hubConnection.DisposeAsync();
        }

        [Fact]
        public async Task creating_a_post_notifies_clients()
        {
            // Arrange
            var testAdmin = await postsByMarkoApiFactory.GetUserByEmailAsync(TestingConstants.TEST_ADMIN_EMAIL);
            var newPost = new PostDto
            {
                AuthorId = testAdmin.Id,
                Title = "Integration Test Post",
                Content = "This is a post created during a SignalR test.",
                Hidden = false
            };

            PostDto? postCreated = null;
            hubConnection!.On<PostDto>("PostCreated", post => postCreated = post);

            // Act
            await client.PostAsJsonAsync($"{controllerPrefix}/create", newPost);
            await postsByMarkoApiFactory.WaitForSignalRPropagation();

            // Assert
            postCreated.Should().NotBeNull();
            postCreated.AuthorId.Should().Be(newPost.AuthorId);
            postCreated.Title.Should().Be(newPost.Title);
            postCreated.Content.Should().Be(newPost.Content);
            postCreated.Hidden.Should().Be(newPost.Hidden);
        }

        [Fact]
        public async Task updating_a_post_notifies_clients()
        {
            // Arrange
            var postRepository = postsByMarkoApiFactory.Resolve<IPostRepository>();
            var allPosts = await postRepository.GetPostsAsync(CancellationToken.None);
            var postToUpdate = allPosts.First();
            var updateRequest = new UpdatePostRequest
            {
                Content = "Updated content",
                Title = "Updated title",
                Hidden = true
            };

            PostDto? postUpdated = null;
            hubConnection!.On<PostDto>("PostUpdated", post => postUpdated = post);

            // Act
            await client.PutAsJsonAsync($"{controllerPrefix}/{postToUpdate.Id}", updateRequest );
            await postsByMarkoApiFactory.WaitForSignalRPropagation();

            // Assert
            postUpdated.Should().NotBeNull();
            postUpdated.Title.Should().Be(updateRequest.Title);
            postUpdated.Content.Should().Be(updateRequest.Content);
            postUpdated.Hidden.Should().Be(updateRequest.Hidden);
        }

        [Fact]
        public async Task deleting_a_post_notifies_clients()
        {
            // Arrange
            var postRepository = postsByMarkoApiFactory.Resolve<IPostRepository>();
            var allPosts = await postRepository.GetPostsAsync(CancellationToken.None);
            var postToDelete = allPosts.First();

            var deletedId = Guid.Empty;

            hubConnection!.On<Guid>("PostDeleted", id => deletedId = id);

            // Act
            await client.DeleteAsync($"{controllerPrefix}/{postToDelete.Id}");
            await postsByMarkoApiFactory.WaitForSignalRPropagation();

            // Assert
            deletedId.Should().NotBe(Guid.Empty);
            deletedId.Should().Be(postToDelete.Id);
        }
    }
}
