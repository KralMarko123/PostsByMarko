using FluentAssertions;
using Newtonsoft.Json;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Host.Data.Models.Responses;
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
        public PostsControllerTests(BaseFixture baseFixture)
        {
            client = baseFixture.client;
        }

        [Fact]
        public async Task should_create_then_update_then_toggle_and_then_finally_delete_a_post()
        {
            var post = new Post { IsHidden = false, PostId = Guid.NewGuid(), Title = "title", Content = "content" };

            await client.PostAsJsonAsync("/create-post", post);

            post.Title = "updated_title";
            post.Content = "updated_content";

            await client.PutAsJsonAsync("/update-post", post);
            await client.PostAsync($"/toggle-post-visibility/{post.PostId}", null);
            await client.PostAsJsonAsync($"/toggle-user-for-post/{post.PostId}", "test_user");

            var result = await client.GetFromJsonAsync<RequestResult>($"/get-post-by-id/{post.PostId}");
            var postDetails = JsonConvert.DeserializeObject<PostDetailsResponse>(result!.Payload!.ToString()!);

            post = postDetails!.Post;

            post!.Title.Should().Be("updated_title");
            post.Content.Should().Be("updated_content");
            post.IsHidden.Should().BeTrue();
            post.AllowedUsers.Should().Contain("test_user");

            await client.DeleteAsync($"/delete-post-by-id/{post.PostId}");

            result = await client.GetFromJsonAsync<RequestResult>("/get-all-posts");

            var allPosts = JsonConvert.DeserializeObject<List<Post>>(result!.Payload!.ToString()!);

            allPosts.Should().NotContain(post);
        }
    }
}