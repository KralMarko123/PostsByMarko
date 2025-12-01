using AutoMapper;
using FluentAssertions;
using Newtonsoft.Json;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Requests;
using PostsByMarko.Host.Data.Repositories.Posts;
using PostsByMarko.Test.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PostsByMarko.IntegrationTests.Controllers
{
    [Collection("IntegrationCollection")]
    public class PostControllerTests : IAsyncLifetime
    {
        private readonly PostsByMarkoApiFactory postsByMarkoApiFactory;
        private readonly HttpClient client;
        private readonly string controllerPrefix = "/api/post";

        public PostControllerTests(PostsByMarkoApiFactory postsByMarkoApiFactory)
        {
            this.postsByMarkoApiFactory = postsByMarkoApiFactory;

            client = postsByMarkoApiFactory.client!;
        }

        public async Task InitializeAsync()
        {
            await postsByMarkoApiFactory.RecreateAndSeedDatabaseAsync();
            await postsByMarkoApiFactory.AuthenticateClientAsync(client);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task should_return_all_posts()
        {
            // Arrange
            var mapper = postsByMarkoApiFactory.Resolve<IMapper>();
            var postRepository = postsByMarkoApiFactory.Resolve<IPostRepository>();

            var allPosts = await postRepository.GetPostsAsync(CancellationToken.None);
            var allPostsDtos = mapper.Map<List<PostDto>>(allPosts);

            // Act
            var response = await client.GetAsync($"{controllerPrefix}/all");
            var responseContent = await response.Content.ReadAsStringAsync();
            var posts = JsonConvert.DeserializeObject<List<PostDto>>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            posts.Should().NotBeNullOrEmpty();
            allPostsDtos.ForEach(postDto =>
            {
                posts.Should().ContainEquivalentOf(postDto);
            });
        }

        [Fact]
        public async Task should_return_a_post()
        {
            // Arrange
            var mapper = postsByMarkoApiFactory.Resolve<IMapper>();
            var postRepository = postsByMarkoApiFactory.Resolve<IPostRepository>();

            var allPosts = await postRepository.GetPostsAsync(CancellationToken.None);
            var postDto = mapper.Map<PostDto>(allPosts.First());

            // Act
            var response = await client.GetAsync($"{controllerPrefix}/{postDto.Id}");
            var responseContent = await response.Content.ReadAsStringAsync();
            var post = JsonConvert.DeserializeObject<PostDto>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            post.Should().NotBeNull();
            post.Should().BeEquivalentTo(postDto);
        }

        [Fact]
        public async Task should_create_a_post()
        {
            // Arrange
            var testAdmin = await postsByMarkoApiFactory.GetUserByEmailAsync(TestingConstants.TEST_ADMIN_EMAIL);
            var createRequest = new CreatePostRequest
            {
                Title = "Post created during integration test",
                Content = "Content for integration test"
            };

            // Act
            var response = await client.PostAsJsonAsync($"{controllerPrefix}", createRequest);
            var responseContent = await response.Content.ReadAsStringAsync();
            var post = JsonConvert.DeserializeObject<PostDto>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            post.Should().NotBeNull();
            post.Id.Should().NotBeEmpty();
            post.Title.Should().Be(createRequest.Title);
            post.Content.Should().Be(createRequest.Content);
            post.AuthorId.Should().Be(testAdmin.Id);
            post.Hidden.Should().BeFalse();
        }

        [Fact]
        public async Task should_update_a_post()
        {
            // Arrange
            var postRepository = postsByMarkoApiFactory.Resolve<IPostRepository>();
            var allPosts = await postRepository.GetPostsAsync(CancellationToken.None);

            var postToUpdate = allPosts.First(); 
            var updateRequest = new UpdatePostRequest
            {
                Title = "Updated Title",
                Content = "Updated Content",
                Hidden = true
            };

            // Act
            var response = await client.PutAsJsonAsync($"{controllerPrefix}/{postToUpdate.Id}", updateRequest);
            var responseContent = await response.Content.ReadAsStringAsync();
            var updatedPost = JsonConvert.DeserializeObject<PostDto>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            updatedPost.Should().NotBeNull();
            updatedPost.Title.Should().Be(updateRequest.Title);
            updatedPost.Content.Should().Be(updateRequest.Content);
            updatedPost.Hidden.Should().Be(updateRequest.Hidden);
            updatedPost.LastUpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task should_delete_a_post()
        {
            // Arrange
            var postRepository = postsByMarkoApiFactory.Resolve<IPostRepository>();
            var allPosts = await postRepository.GetPostsAsync(CancellationToken.None);
            var postToDelete = allPosts.First();

            // Act
            var response = await client.DeleteAsync($"{controllerPrefix}/{postToDelete.Id}");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var deletedPost = await postRepository.GetPostByIdAsync(postToDelete.Id, CancellationToken.None);

            deletedPost.Should().BeNull();
        }
    }
}