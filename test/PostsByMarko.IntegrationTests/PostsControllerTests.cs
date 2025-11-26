using AutoMapper;
using FluentAssertions;
using Newtonsoft.Json;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Requests;
using PostsByMarko.Host.Data.Entities;
using PostsByMarko.Host.Data.Repositories.Posts;
using PostsByMarko.Test.Shared.Constants;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PostsByMarko.IntegrationTests
{
    [Collection("Sequential Collection")]
    public class PostsControllerTests
    {
        private readonly HttpClient client;
        private readonly User testAdmin = TestingConstants.TEST_ADMIN;
        private readonly IPostRepository postRepository;
        private readonly IMapper mapper;
        private readonly string controllerPrefix = "/api/post";

        public PostsControllerTests(PostsByMarkoApiFactory postsByMarkoApiFactory)
        {
            client = postsByMarkoApiFactory.authenticatedClient!;
            postRepository = postsByMarkoApiFactory.postRepository!;
            mapper = postsByMarkoApiFactory.mapper!;
        }

        [Fact]
        public async Task should_return_all_posts()
        {
            // Arrange
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
            var newPost = new PostDto
            {
                AuthorId = testAdmin.Id,
                Title = "Integration Test Post",
                Content = "This is a post created during an integration test.",
                Hidden = false
            };

            // Act
            var response = await client.PostAsJsonAsync($"{controllerPrefix}/create", newPost);
            var responseContent = await response.Content.ReadAsStringAsync();
            var post = JsonConvert.DeserializeObject<PostDto>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            post.Should().NotBeNull();
            post.Id.Should().NotBeEmpty();
            post.Title.Should().Be(newPost.Title);
            post.Content.Should().Be(newPost.Content);
            post.AuthorId.Should().Be(newPost.AuthorId);
            post.Hidden.Should().Be(newPost.Hidden);
        }

        [Fact]
        public async Task should_update_a_post()
        {
            // Arrange
            var allPosts = await postRepository.GetPostsAsync(CancellationToken.None);
            var postToUpdate = allPosts.First();
            var updateRequest = new UpdatePostRequest
            {
                Title = "Updated Title",
                Content = "Updated Content",
                Hidden = true
            };

            // Act
            var response = await client.PutAsJsonAsync($"{controllerPrefix}/update/{postToUpdate.Id}", updateRequest);
            var responseContent = await response.Content.ReadAsStringAsync();
            var updatedPost = JsonConvert.DeserializeObject<PostDto>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            updatedPost.Should().NotBeNull();
            updatedPost.Title.Should().Be(updateRequest.Title);
            updatedPost.Content.Should().Be(updateRequest.Content);
            updatedPost.Hidden.Should().Be(updateRequest.Hidden);
            updatedPost.LastUpdatedDate.Should().BeAfter(postToUpdate.LastUpdatedDate);
        }

        [Fact]
        public async Task should_delete_a_post()
        {
            // Arrange
            var allPosts = await postRepository.GetPostsAsync(CancellationToken.None);
            var postToDelete = allPosts.First();

            // Act
            var response = await client.DeleteAsync($"{controllerPrefix}/delete/{postToDelete.Id}");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var deletedPost = await postRepository.GetPostByIdAsync(postToDelete.Id, CancellationToken.None);

            deletedPost.Should().BeNull();
        }

    }
}