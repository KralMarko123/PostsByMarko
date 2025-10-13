using FluentAssertions;
using Moq;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Host.Data.Models.Responses;
using PostsByMarko.Host.Data.Repos.Posts;
using PostsByMarko.Host.Data.Repos.Users;
using PostsByMarko.Host.Services;
using System.Net;

namespace PostsByMarko.UnitTests
{
    public class PostsServiceTests
    {
        private readonly PostsService service;
        private readonly Mock<IPostsRepository> postsRepositoryMock = new Mock<IPostsRepository>();
        private readonly Mock<IUsersRepository> usersRepositoryMock = new Mock<IUsersRepository>();

        public PostsServiceTests()
        {
            service = new PostsService(postsRepositoryMock.Object, usersRepositoryMock.Object);
        }

        [Fact]
        public async Task get_all_posts_should_return_ok_with_payload_of_all_posts()
        {
            // Arrange
            var postsToReturn = Enumerable.Range(0, 10).Select((p) => new Post($"Ttile {p}", $"Content {p}")).ToList();
            var user = new RequestUser { Id = Guid.NewGuid().ToString(), Email = "test_user@test.com", Roles = ["Admin"] };

            postsRepositoryMock.Setup(r => r.GetPostsAsync()).ReturnsAsync(postsToReturn);

            // Act
            var result = await service.GetAllPostsAsync(user);
            var resultPayload = result.Payload as List<Post>;

            // Assert
            resultPayload.Should().BeEquivalentTo(postsToReturn);
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task get_all_posts_should_return_ok_with_payload_of_filtered_posts()
        {
            // Arrange
            var user = new RequestUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = "test_user@test.com",
                Roles = ["User"]
            };

            var postsToReturn = new List<Post>
            {
               new Post { IsHidden= false },
               new Post { IsHidden = true, AuthorId = user.Id },
               new Post { IsHidden = true }
            };

            var postToBeFiltered = postsToReturn.Last();

            postsRepositoryMock.Setup(r => r.GetPostsAsync()).ReturnsAsync(postsToReturn);

            // Act
            var result = await service.GetAllPostsAsync(user);
            var resultPayload = result.Payload as List<Post>;

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            resultPayload.Should().NotContain(postToBeFiltered);
        }

        [Fact]
        public async Task get_post_by_id_should_return_ok_with_payload_of_post_when_post_exists()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var postToReturn = new Post { Id = Guid.NewGuid().ToString(), AuthorId = userId };
            var user = new User("test_user@test.com") { Id = userId, Email = "test_user@test.com", FirstName = "test", LastName = "user", Posts = [postToReturn] };

            usersRepositoryMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);
            postsRepositoryMock.Setup(r => r.GetPostByIdAsync(postToReturn.Id.ToString())).ReturnsAsync(postToReturn);

            // Act
            var result = await service.GetPostByIdAsync(postToReturn.Id, new RequestUser { Id = user.Id, Email = user.Email, Roles = ["Admin"] });
            var resultPayload = result.Payload as Post;

            // Assert
            resultPayload.Should().BeEquivalentTo(postToReturn);
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task get_post_by_id_should_return_not_found_and_appropriate_message_when_post_does_not_exist()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString().ToString();
            var nonexistentPostId = Guid.NewGuid().ToString().ToString();
            var user = new User("test_user@test.com");

            usersRepositoryMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);
            postsRepositoryMock.Setup(r => r.GetPostByIdAsync(It.IsAny<string>())).ReturnsAsync(() => null);

            // Act
            var result = await service.GetPostByIdAsync(nonexistentPostId, new RequestUser { Id = user.Id, Email = user.Email, Roles = ["Admin"] });

            // Assert
            result.Message.Should().Be($"Post with Id: {nonexistentPostId} was not found");
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task get_post_by_id_should_return_unauthorized_and_appropriate_message_when_post_is_hidden()
        {
            // Arrange
            var postToReturn = new Post { Id = Guid.NewGuid().ToString(), IsHidden = true };
            var user = new User("test_user@test.com");

            usersRepositoryMock.Setup(r => r.GetUserByIdAsync(user.Id)).ReturnsAsync(user);
            postsRepositoryMock.Setup(r => r.GetPostByIdAsync(postToReturn.Id)).ReturnsAsync(postToReturn);

            // Act
            var result = await service.GetPostByIdAsync(postToReturn.Id, new RequestUser { Id = user.Id, Email = user.Email, Roles = ["User"] });

            // Assert
            result.Message.Should().Be($"Post with Id: {postToReturn.Id} is hidden");
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task create_post_should_return_post_and_appropriate_message_when_post_is_created()
        {
            // Arrange
            var postToCreate = new Post { Title = "Test Title", Content = "Test Content" };
            var user = new User("test_user@test.com");

            usersRepositoryMock.Setup(r => r.GetUserByEmailAsync(user.Email)).ReturnsAsync(user);
            postsRepositoryMock.Setup(r => r.CreatePostAsync(postToCreate)).ReturnsAsync(postToCreate);

            // Act
            var result = await service.CreatePostAsync(postToCreate, new RequestUser { Id = user.Id, Email = user.Email });

            // Assert
            result.Message.Should().Be("Post was created successfully");
            result.StatusCode.Should().Be(HttpStatusCode.Created);
            result.Payload.Should().Be(postToCreate);
        }

        [Fact]
        public async Task create_post_should_return_bad_request_and_appropriate_message_when_post_fails_to_be_created()
        {
            // Arrange
            var postToCreate = new Post { Title = "Test Title", Content = "Test Content" };
            var user = new User("test_user@test.com");

            usersRepositoryMock.Setup(r => r.GetUserByEmailAsync(user.Email)).ReturnsAsync(user);
            postsRepositoryMock.Setup(r => r.CreatePostAsync(postToCreate)).ThrowsAsync(new Exception());

            // Act
            var result = await service.CreatePostAsync(postToCreate, new RequestUser { Id = user.Id, Email = user.Email });

            // Assert
            result.Message.Should().Be("Error during post creation");
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task create_post_should_return_bad_request_and_appropriate_message_when_post_is_created_with_no_title_and_content()
        {
            // Arrange
            var postToCreate = new Post();
            var user = new User("test_user@test.com");

            // Act
            var result = await service.CreatePostAsync(postToCreate, new RequestUser { Id = user.Id, Email = user.Email });

            // Assert
            result.Message.Should().Be("Error during post creation");
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task update_post_should_return_ok_with_appropriate_message_when_post_is_updated()
        {
            // Arrange
            var user = new User("test_user@test.com");
            var postToUpdate = new Post { Id = Guid.NewGuid().ToString(), Title = "Test Title", Content = "Test Content", AuthorId = user.Id };
            var updatedPost = new Post { Id = postToUpdate.Id, Title = "Updated Title", Content = "Updated Content", AuthorId = user.Id, LastUpdatedDate = DateTime.UtcNow };

            postsRepositoryMock.Setup(r => r.GetPostByIdAsync(postToUpdate.Id.ToString())).ReturnsAsync(postToUpdate);
            postsRepositoryMock.Setup(r => r.UpdatePostAsync(postToUpdate)).ReturnsAsync(() => true);

            // Act
            var result = await service.UpdatePostAsync(updatedPost, new RequestUser { Id = user.Id, Email = user.Email, Roles = ["Admin"] });

            // Assert
            postToUpdate.Should().BeEquivalentTo(updatedPost, options => options
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromMilliseconds(1000)))
            .WhenTypeIs<DateTime>());
            result.Message.Should().Be("Post was updated successfully");
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task update_post_should_return_bad_request_with_appropriate_message_when_post_is_not_updated()
        {
            // Arrange
            var postToUpdate = new Post { Id = Guid.NewGuid().ToString(), Title = "Test Title", Content = "Test Content" };
            var updatedPost = new Post { Id = postToUpdate.Id, Title = "Updated Title", Content = "Updated Content" };
            var user = new User("test_user@test.com");

            postsRepositoryMock.Setup(r => r.GetPostByIdAsync(postToUpdate.Id.ToString())).ReturnsAsync(postToUpdate);
            postsRepositoryMock.Setup(r => r.UpdatePostAsync(postToUpdate)).ReturnsAsync(() => false);

            // Act
            var result = await service.UpdatePostAsync(updatedPost, new RequestUser { Id = user.Id, Email = user.Email, Roles = ["Admin"] });

            // Assert
            result.Message.Should().Be("Error during post update");
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task update_post_should_return_bad_request_with_appropriate_message_when_post_cannot_be_updated()
        {
            // Arrange
            var postToUpdate = new Post { Id = Guid.NewGuid().ToString(), Title = "Test Title", Content = "Test Content" };
            var updatedPost = new Post { Id = postToUpdate.Id, Title = "Updated Title", Content = "Updated Content" };
            var user = new User("test_user@test.com");

            postsRepositoryMock.Setup(r => r.GetPostByIdAsync(postToUpdate.Id.ToString())).ReturnsAsync(postToUpdate);

            // Act
            var result = await service.UpdatePostAsync(updatedPost, new RequestUser { Id = user.Id, Email = user.Email, Roles = ["User"] });

            // Assert
            result.Message.Should().Be("Error during post update");
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task update_post_should_return_bad_request_with_appropriate_message_when_post_does_not_exist()
        {
            // Arrange
            var postToUpdate = new Post { Id = Guid.NewGuid().ToString(), Title = "Test Title", Content = "Test Content" };
            var updatedPost = new Post { Id = postToUpdate.Id, Title = "Updated Title", Content = "Updated Content" };
            var user = new User("test_user@test.com");

            postsRepositoryMock.Setup(r => r.GetPostByIdAsync(It.IsAny<string>())).ReturnsAsync(() => null);

            // Act
            var result = await service.UpdatePostAsync(updatedPost, new RequestUser { Id = user.Id, Email = user.Email, Roles = ["User"] });

            // Assert
            result.Message.Should().Be("Error during post update");
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task delete_post_should_return_ok_with_appropriate_message_when_post_is_deleted()
        {
            // Arrange
            var user = new User("test_user@test.com");
            var postToDelete = new Post { Id = Guid.NewGuid().ToString(), AuthorId = user.Id };

            postsRepositoryMock.Setup(r => r.GetPostByIdAsync(postToDelete.Id.ToString())).ReturnsAsync(postToDelete);
            postsRepositoryMock.Setup(r => r.DeletePostAsync(postToDelete)).ReturnsAsync(() => true);

            // Act
            var result = await service.DeletePostByIdAsync(postToDelete.Id.ToString(), new RequestUser { Id = user.Id, Email = user.Email });

            // Assert
            result.Message.Should().Be("Post was deleted successfully");
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task delete_post_should_return_bad_request_with_appropriate_message_when_post_is_not_deleted()
        {
            // Arrange
            var user = new User("test_user@test.com");
            var postToDelete = new Post { Id = Guid.NewGuid().ToString(), AuthorId = user.Id };

            postsRepositoryMock.Setup(r => r.GetPostByIdAsync(postToDelete.Id.ToString())).ReturnsAsync(postToDelete);
            postsRepositoryMock.Setup(r => r.DeletePostAsync(postToDelete)).ReturnsAsync(() => false);

            // Act
            var result = await service.DeletePostByIdAsync(postToDelete.Id.ToString(), new RequestUser { Id = user.Id, Email = user.Email });

            // Assert
            result.Message.Should().Be("Error during post deletion");
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task delete_post_should_return_bad_request_with_appropriate_message_when_post_cannot_be_deleted()
        {
            // Arrange
            var postToDelete = new Post { Id = Guid.NewGuid().ToString(), AuthorId = Guid.NewGuid().ToString() };
            var user = new User("test_user@test.com");

            postsRepositoryMock.Setup(r => r.GetPostByIdAsync(postToDelete.Id.ToString())).ReturnsAsync(postToDelete);

            // Act
            var result = await service.DeletePostByIdAsync(postToDelete.Id.ToString(), new RequestUser {Id = user.Id, Email = user.Email, Roles = ["User"] });

            // Assert
            result.Message.Should().Be("Error during post deletion");
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task delete_post_should_return_bad_request_with_appropriate_message_when_post_does_not_exist()
        {
            // Arrange
            var postToDelete = new Post { Id = Guid.NewGuid().ToString(), AuthorId = Guid.NewGuid().ToString().ToString() };
            var user = new User("test_user@test.com");

            postsRepositoryMock.Setup(r => r.GetPostByIdAsync(It.IsAny<string>())).ReturnsAsync(() => null);

            // Act
            var result = await service.DeletePostByIdAsync(postToDelete.Id.ToString(), new RequestUser {Id = user.Id, Email = user.Email, Roles = ["User"] });

            // Assert
            result.Message.Should().Be("Error during post deletion");
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task toggling_post_visibility_should_return_ok_with_apppropriate_message_when_post_is_toggled()
        {
            // Arrange
            var user = new User("test_user@test.com");
            var postToToggle = new Post { IsHidden = false, Id = Guid.NewGuid().ToString(), AuthorId = user.Id };

            postsRepositoryMock.Setup(r => r.GetPostByIdAsync(postToToggle.Id.ToString())).ReturnsAsync(postToToggle);
            postsRepositoryMock.Setup(r => r.UpdatePostAsync(postToToggle)).ReturnsAsync(() => true);

            // Act
            var result = await service.TogglePostVisibilityAsync(postToToggle.Id.ToString(), new RequestUser {Id = user.Id, Email = user.Email, Roles = ["Admin"] });

            // Assert
            postToToggle.IsHidden.Should().BeTrue();
            result.Message.Should().Be("Post visibility was toggled successfully");
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task toggling_post_visibility_should_return_bad_request_with_apppropriate_message_when_post_is_not_toggled()
        {
            // Arrange
            var user = new User("test_user@test.com");
            var postToToggle = new Post("Some Title", "Some Content");
            postToToggle.AuthorId = user.Id;

            postsRepositoryMock.Setup(r => r.GetPostByIdAsync(postToToggle.Id.ToString())).ReturnsAsync(postToToggle);
            postsRepositoryMock.Setup(r => r.UpdatePostAsync(postToToggle)).ReturnsAsync(() => false);

            // Act
            var result = await service.TogglePostVisibilityAsync(postToToggle.Id.ToString(), new RequestUser {Id = user.Id, Email = user.Email, Roles = ["Admin"] });

            // Assert
            result.Message.Should().Be("Error during post visibility toggle");
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task toggling_post_visibility_should_return_unauthorized_with_apppropriate_message_when_post_cannot_be_toggled()
        {
            // Arrange
            var user = new User("test_user@test.com");
            var postToToggle = new Post { IsHidden = false, Id = Guid.NewGuid().ToString(), AuthorId = Guid.NewGuid().ToString().ToString() };

            postsRepositoryMock.Setup(r => r.GetPostByIdAsync(postToToggle.Id.ToString())).ReturnsAsync(postToToggle);

            // Act
            var result = await service.TogglePostVisibilityAsync(postToToggle.Id.ToString(), new RequestUser {Id = user.Id, Email = user.Email, Roles = ["User"] });

            // Assert
            postToToggle.IsHidden.Should().BeFalse();
            result.Message.Should().Be("Unauthorized to toggle Post's visibility");
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
