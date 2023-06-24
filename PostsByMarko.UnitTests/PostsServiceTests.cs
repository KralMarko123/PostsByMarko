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
        public async Task get_all_posts_should_return_payload_with_all_posts()
        {
            // Arrange
            var postsToReturn = new List<Post>{
               new Post { PostId = Guid.NewGuid() },
               new Post { PostId = Guid.NewGuid() },
               new Post { PostId = Guid.NewGuid() }
            };

            var user = new RequestUser
            {
                UserId = Guid.NewGuid().ToString(),
                Username = "test_user",
                UserRoles = new List<string> { "Admin" }
            };

            postsRepositoryMock.Setup(r => r.GetPostsAsync()).ReturnsAsync(postsToReturn);

            // Act
            var result = await service.GetAllPostsAsync(user);
            var resultPayload = result.Payload as List<Post>;

            // Assert
            resultPayload.Should().BeEquivalentTo(postsToReturn);
        }

        [Fact]
        public async Task get_all_posts_should_return_payload_with_filtered_posts()
        {
            // Arrange
            var user = new RequestUser
            {
                UserId = Guid.NewGuid().ToString(),
                Username = "test_user",
                UserRoles = new List<string> { "User" }
            };

            var postsToReturn = new List<Post>
            {
               new Post { IsHidden= true, AllowedUsers = new List<string>{ "test_user" } },
               new Post { IsHidden = true, UserId = user.UserId },
               new Post { IsHidden = true }
            };

            var postToBeFiltered = postsToReturn.Last();

            postsRepositoryMock.Setup(r => r.GetPostsAsync()).ReturnsAsync(postsToReturn);

            // Act
            var result = await service.GetAllPostsAsync(user);
            var resultPayload = result.Payload as List<Post>;

            // Assert
            resultPayload.Should().NotContain(postToBeFiltered);
        }

        [Fact]
        public async Task get_post_by_id_should_return_payload_with_post_when_post_exists()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var postToReturn = new Post { PostId = Guid.NewGuid(), UserId = userId };
            var user = new User { Id = userId, UserName = "test_user", FirstName = "test", LastName = "user", Posts = new List<Post> { postToReturn } };

            usersRepositoryMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);
            postsRepositoryMock.Setup(r => r.GetPostByIdAsync(postToReturn.PostId.ToString())).ReturnsAsync(postToReturn);

            // Act
            var result = await service.GetPostByIdAsync(postToReturn.PostId.ToString(), new RequestUser { UserId = user.Id, Username = user.UserName, UserRoles = new List<string> { "Admin" } });
            var resultPayload = result.Payload as PostDetailsResponse;

            // Assert
            resultPayload!.Post.Should().BeEquivalentTo(postToReturn);
        }

        [Fact]
        public async Task get_post_by_id_should_return_not_found_and_appropriate_message_when_post_does_not_exist()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var nonexistentPostId = Guid.NewGuid().ToString();
            var user = new User { Id = userId, UserName = "test_user", FirstName = "test", LastName = "user" };

            usersRepositoryMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);
            postsRepositoryMock.Setup(r => r.GetPostByIdAsync(It.IsAny<string>())).ReturnsAsync(() => null);

            // Act
            var result = await service.GetPostByIdAsync(nonexistentPostId, new RequestUser { UserId = user.Id, Username = user.UserName, UserRoles = new List<string> { "Admin" } });

            // Assert
            result.Message.Should().Be($"Post with Id: {nonexistentPostId} was not found");
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task get_post_by_id_should_return_unauthorized_and_appropriate_message_when_post_is_hidden()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var postToReturn = new Post { PostId = Guid.NewGuid(), IsHidden = true, AllowedUsers = new List<string> { "other_user" } };
            var user = new User { Id = userId, UserName = "test_user", FirstName = "test", LastName = "user" };

            usersRepositoryMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);
            postsRepositoryMock.Setup(r => r.GetPostByIdAsync(postToReturn.PostId.ToString())).ReturnsAsync(postToReturn);

            // Act
            var result = await service.GetPostByIdAsync(postToReturn.PostId.ToString(), new RequestUser { UserId = user.Id, Username = user.UserName, UserRoles = new List<string> { "User" } });

            // Assert
            result.Message.Should().Be($"Post with Id: {postToReturn.PostId} is hidden");
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
