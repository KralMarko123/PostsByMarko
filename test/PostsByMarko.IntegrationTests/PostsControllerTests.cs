using FluentAssertions;
using Newtonsoft.Json;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Host.Data.Models.Responses;
using PostsByMarko.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace PostsByMarko.IntegrationTests
{
    [Collection("Integration Collection")]
    public class PostsControllerTests
    {
        private readonly HttpClient client;
        public PostsControllerTests(PostsByMarkoApiFactory postsByMarkoApiFactory)
        {
            client = postsByMarkoApiFactory.client!;
        }

        [Fact]
        public async Task should_create_then_update_then_toggle_and_then_finally_delete_a_post()
        {
            // Arrange
            var post = new Post { Id = Guid.NewGuid().ToString(), IsHidden = false, Title = "title", Content = "content", AuthorId = Guid.NewGuid().ToString() };
            var testEmail = TestingConstants.TEST_USER.Email;

            // Act
            await client.PostAsJsonAsync("/createPost", post);

            post.Title = "updated_title";
            post.Content = "updated_content";

            await client.PutAsJsonAsync("/updatePost", post);
            await client.PostAsync($"/togglePostVisibility/{post.Id}", null);
            await client.PostAsJsonAsync($"/toggleUserForPost/{post.Id}", testEmail);

            var result = await client.GetFromJsonAsync<RequestResult>($"/getPost/{post.Id}");
            var postDetails = JsonConvert.DeserializeObject<PostDetailsResponse>(result!.Payload!.ToString()!);

            post = postDetails!.Post;

            // Assert
            post!.Title.Should().Be("updated_title");
            post.Content.Should().Be("updated_content");
            post.IsHidden.Should().BeTrue();
            post.AllowedUsers.Should().Contain(testEmail);

            await client.DeleteAsync($"/deletePost/{post.Id}");

            result = await client.GetFromJsonAsync<RequestResult>("/getAllPosts");

            var allPosts = JsonConvert.DeserializeObject<List<Post>>(result!.Payload!.ToString()!);

            allPosts.Should().NotContain(post);
        }
    }
}