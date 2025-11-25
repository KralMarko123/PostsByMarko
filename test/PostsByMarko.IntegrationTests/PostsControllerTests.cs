//using Bogus;
//using FluentAssertions;
//using Newtonsoft.Json;
//using PostsByMarko.Host.Data.Entities;
//using PostsByMarko.Test.Shared.Constants;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Json;
//using System.Threading.Tasks;
//using Xunit;

//namespace PostsByMarko.IntegrationTests
//{
//    [Collection("Integration Collection")]
//    public class PostsControllerTests
//    {
//        private readonly HttpClient client;
//        private readonly User testAdmin = TestingConstants.TEST_ADMIN;

//        public PostsControllerTests(PostsByMarkoApiFactory postsByMarkoApiFactory)
//        {
//            client = postsByMarkoApiFactory.client!;
//        }

//        [Fact]
//        public async Task should_return_all_posts()
//        {
//            // Arrange
//            var randomTitle = new Faker().Commerce.ProductName();
//            var posts = new List<Post>() { new Post(randomTitle, "Content"), new Post(randomTitle, "Materials") };

//            foreach (var post in posts)
//            {
//                await client.PostAsJsonAsync("/createPost", post);
//            }

//            // Act
//            var requestResult = await client.GetFromJsonAsync<RequestResult>("/getAllPosts");
//            var allPosts = JsonConvert.DeserializeObject<List<Post>>(requestResult.Payload.ToString());

//            // Assert
//            requestResult.Should().NotBeNull();
//            requestResult.StatusCode.Should().Be(HttpStatusCode.OK);

//            allPosts.Should().NotBeNullOrEmpty();
//            allPosts.Select(p => p.Title).Should().Contain(randomTitle);
//        }

//        [Fact]
//        public async Task should_create_a_post()
//        {
//            // Arrange
//            var post = new Post("Title", "Content");

//            // Act
//            var response = await client.PostAsJsonAsync("/createPost", post);
//            var requestResult = await response.Content.ReadFromJsonAsync<RequestResult>();
//            var createdPost = JsonConvert.DeserializeObject<Post>(requestResult.Payload.ToString());

//            // Assert
//            requestResult.Should().NotBeNull();
//            requestResult.StatusCode.Should().Be(HttpStatusCode.Created);
//            requestResult.Message.Should().Be("Post was created successfully");

//            createdPost.Should().NotBeNull();
//            createdPost.Title.Should().Be(post.Title);
//            createdPost.Content.Should().Be(post.Content);
//            createdPost.AuthorId.Should().Be(testAdmin.Id);
//        }

//        [Fact]
//        public async Task should_update_a_post()
//        {
//            // Arrange
//            var post = new Post("Title", "Content");
//            await client.PostAsJsonAsync("/createPost", post);

//            post.Title = "Updated title";
//            post.Content = "Updated content";

//            // Act
//            var response = await client.PutAsJsonAsync("/updatePost", post);
//            var requestResult = await response.Content.ReadFromJsonAsync<RequestResult>();

//            var postResponse = await client.GetFromJsonAsync<RequestResult>($"/getPost/{post.Id}");
//            var updatedPost = JsonConvert.DeserializeObject<Post>(postResponse!.Payload!.ToString()!);

//            // Assert
//            requestResult.Should().NotBeNull();
//            requestResult.StatusCode.Should().Be(HttpStatusCode.OK);
//            requestResult.Message.Should().Be("Post was updated successfully");

//            updatedPost.Should().NotBeNull();
//            updatedPost.Title.Should().Be(post.Title);
//            updatedPost.Content.Should().Be(post.Content);
//        }

//        [Fact]
//        public async Task should_hide_a_post()
//        {
//            // Arrange
//            var post = new Post("Title", "Content");
//            await client.PostAsJsonAsync("/createPost", post);

//            // Act
//            var response = await client.PostAsync($"/togglePostVisibility/{post.Id}", null);
//            var requestResult = await response.Content.ReadFromJsonAsync<RequestResult>();

//            var postResponse = await client.GetFromJsonAsync<RequestResult>($"/getPost/{post.Id}");
//            var hiddenPost = JsonConvert.DeserializeObject<Post>(postResponse!.Payload!.ToString()!);

//            // Assert
//            requestResult.Should().NotBeNull();
//            requestResult.StatusCode.Should().Be(HttpStatusCode.OK);
//            requestResult.Message.Should().Be("Post visibility was toggled successfully");

//            hiddenPost.Should().NotBeNull();
//            hiddenPost.IsHidden.Should().BeTrue();
//        }

//        [Fact]
//        public async Task should_delete_a_post()
//        {
//            // Arrange
//            var post = new Post("Title", "Content");
//            await client.PostAsJsonAsync("/createPost", post);

//            // Act
//            var response = await client.DeleteAsync($"/deletePost/{post.Id}");
//            var requestResult = await response.Content.ReadFromJsonAsync<RequestResult>();

//            var postResponse = await client.GetFromJsonAsync<RequestResult>($"/getPost/{post.Id}");

//            // Assert
//            requestResult.Should().NotBeNull();
//            requestResult.StatusCode.Should().Be(HttpStatusCode.OK);
//            requestResult.Message.Should().Be("Post was deleted successfully");

//            postResponse.Should().NotBeNull();
//            postResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
//            postResponse.Message.Should().Be($"Post with Id: {post.Id} was not found");
//        }

//        [Fact]
//        public async Task should_return_post_author()
//        {
//            // Arrange
//            var post = new Post("Title", "Content");
//            await client.PostAsJsonAsync("/createPost", post);

//            // Act
//            var response = await client.GetFromJsonAsync<RequestResult>($"/getPostAuthor/{post.Id}");
//            var postAuthor = JsonConvert.DeserializeObject<AuthorDetailsResponse>(response!.Payload!.ToString()!);

//            // Assert
//            response.Should().NotBeNull();
//            response.StatusCode.Should().Be(HttpStatusCode.OK);

//            postAuthor.Should().NotBeNull();
//            postAuthor.Should().BeEquivalentTo(new AuthorDetailsResponse(testAdmin));
//        }

//        [Fact]
//        public async Task should_return_not_found_if_post_does_not_exist()
//        {
//            // Arrange
//            var randomId = Guid.NewGuid().ToString();

//            // Act
//            var postResponse = await client.GetFromJsonAsync<RequestResult>($"/getPost/{randomId}");
//            var authorResponse = await client.GetFromJsonAsync<RequestResult>($"/getPostAuthor/{randomId}");

//            // Assert
//            postResponse.Should().NotBeNull();
//            postResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
//            postResponse.Message.Should().Be($"Post with Id: {randomId} was not found");
//            postResponse.Payload.Should().BeNull();

//            authorResponse.Should().NotBeNull();
//            authorResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
//            authorResponse.Message.Should().Be($"Post with Id: {randomId} was not found");
//            authorResponse.Payload.Should().BeNull();
//        }
//    }
//}