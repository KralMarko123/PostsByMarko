using FluentAssertions;
using Newtonsoft.Json;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Shared.Constants;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PostsByMarko.IntegrationTests
{
    [Collection("Integration Collection")]
    public class PostsControllerTests
    {
        private readonly HttpClient client;
        private readonly User testUser = TestingConstants.TEST_USER;
        private readonly ITestOutputHelper outputHelper;

        public PostsControllerTests(PostsByMarkoApiFactory postsByMarkoApiFactory, ITestOutputHelper outputHelper)
        {
            client = postsByMarkoApiFactory.client!;
            this.outputHelper = outputHelper;
        }

        [Fact]
        public async Task should_successfully_create_a_post()
        {
            // Arrange
            var post = new Post("Title", "Content");

            // Act
            var response = await client.PostAsJsonAsync("/createPost", post);

            await OutputResponseToTestLogs(response);

            var requestResult = await response.Content.ReadFromJsonAsync<RequestResult>();
            var createdPost = JsonConvert.DeserializeObject<Post>(requestResult.Payload.ToString());

            // Assert
            requestResult.StatusCode.Should().Be(HttpStatusCode.Created);
            requestResult.Message.Should().Be("Post was created successfully");
            createdPost!.Title.Should().Be(post.Title);
            createdPost.Content.Should().Be(post.Content);
            createdPost.AuthorId.Should().Be(testUser.Id);
        }

        [Fact]
        public async Task should_successfully_update_a_post()
        {
            // Arrange
            var post = new Post("Title", "Content");
            await client.PostAsJsonAsync("/createPost", post);

            post.Title = "Updated title";
            post.Content = "Updated content";

            // Act
            var response = await client.PutAsJsonAsync("/updatePost", post);

            await OutputResponseToTestLogs(response);


            var requestResult = await response.Content.ReadFromJsonAsync<RequestResult>();

            var postResponse = await client.GetFromJsonAsync<RequestResult>($"/getPost/{post.Id}");
            var updatedPost = JsonConvert.DeserializeObject<Post>(postResponse!.Payload!.ToString()!);

            // Assert
            requestResult.StatusCode.Should().Be(HttpStatusCode.OK);
            requestResult.Message.Should().Be("Post was updated successfully");
            updatedPost!.Title.Should().Be(post.Title);
            updatedPost.Content.Should().Be(post.Content);
        }

        [Fact]
        public async Task should_successfully_hide_a_post()
        {
            // Arrange
            var post = new Post("Title", "Content");
            await client.PostAsJsonAsync("/createPost", post);

            // Act
            var response = await client.PostAsync($"/togglePostVisibility/{post.Id}", null);

            await OutputResponseToTestLogs(response);


            var requestResult = await response.Content.ReadFromJsonAsync<RequestResult>();

            var postResponse = await client.GetFromJsonAsync<RequestResult>($"/getPost/{post.Id}");
            var hiddenPost = JsonConvert.DeserializeObject<Post>(postResponse!.Payload!.ToString()!);

            // Assert
            requestResult.StatusCode.Should().Be(HttpStatusCode.OK);
            requestResult.Message.Should().Be("Post visibility was toggled successfully");
            hiddenPost!.IsHidden.Should().BeTrue();
        }

        [Fact]
        public async Task should_successfully_delete_a_post()
        {
            // Arrange
            var post = new Post("Title", "Content");
            await client.PostAsJsonAsync("/createPost", post);

            // Act
            var response = await client.DeleteAsync($"/deletePost/{post.Id}");


            await OutputResponseToTestLogs(response);


            var requestResult = await response.Content.ReadFromJsonAsync<RequestResult>();

            var postResponse = await client.GetFromJsonAsync<RequestResult>($"/getPost/{post.Id}");

            // Assert
            requestResult.StatusCode.Should().Be(HttpStatusCode.OK);
            requestResult.Message.Should().Be("Post was deleted successfully");
            
            postResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            postResponse.Message.Should().Be($"Post with Id: {post.Id} was not found");
        }

        private async Task OutputResponseToTestLogs(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

            outputHelper.WriteLine($"Response was: {response}");
            outputHelper.WriteLine($"Response content was: {responseContent}");
        }
    }
}