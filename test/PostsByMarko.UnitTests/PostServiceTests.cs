using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Moq;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Hubs;
using PostsByMarko.Host.Application.Hubs.Client;
using PostsByMarko.Host.Application.Interfaces;
using PostsByMarko.Host.Application.Requests;
using PostsByMarko.Host.Application.Services;
using PostsByMarko.Host.Data.Entities;
using PostsByMarko.Host.Data.Repositories.Posts;
using PostsByMarko.Host.Data.Repositories.Users;

namespace PostsByMarko.UnitTests
{
    public class PostServiceTests
    {
        private readonly PostService postService;
        private readonly Mock<IPostRepository> postsRepositoryMock = new();
        private readonly Mock<IUserRepository> userRepositoryMock = new();
        private readonly Mock<ICurrentRequestAccessor> currentRequestAccessorMock = new();
        private readonly Mock<IMapper> mapperMock = new();
        private readonly Mock<IHubContext<PostHub, IPostClient>> postHubMock = new();
        private readonly Mock<IPostClient> postClientMock = new();

        public PostServiceTests()
        {
            postService = new PostService(postsRepositoryMock.Object, 
                userRepositoryMock.Object, 
                currentRequestAccessorMock.Object,
                mapperMock.Object,
                postHubMock.Object);
        }

        [Fact]
        public async Task get_all_posts_should_return_all_posts()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "test@test.com" };
            var userRoles = new List<string> { "Admin" };
            var posts = new List<Post>
            {
                new Post { Title = "Some Title" },
                new Post { Title = "Other Title" }
            };
            var postDtos = new List<PostDto>
            {
                new PostDto { Title = "Some Title" },
                new PostDto { Title = "Other Title" }
            };

            currentRequestAccessorMock.Setup(c => c.Id).Returns(user.Id);
            userRepositoryMock.Setup(s => s.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            userRepositoryMock.Setup(s => s.GetRolesForUserAsync(user)).ReturnsAsync(userRoles);
            postsRepositoryMock.Setup(r => r.GetPostsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(posts);
            mapperMock.Setup(m => m.Map<List<PostDto>>(posts)).Returns(postDtos);

            // Act
            var result = await postService.GetAllPostsAsync(CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(postDtos);
        }

        [Fact]
        public async Task get_all_posts_should_return_filtered_posts()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "test@test.com" };
            var userRoles = new List<string> { "User" };
            var posts = new List<Post>
            {
                new Post { Title = "Some Title" },
                new Post { Hidden = true }
            };
            var postDtos = new List<PostDto>
            {
                new PostDto { Title = "Some Title" },
                new PostDto { Hidden = true }
            };

            currentRequestAccessorMock.Setup(c => c.Id).Returns(user.Id);
            userRepositoryMock.Setup(s => s.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            userRepositoryMock.Setup(s => s.GetRolesForUserAsync(user)).ReturnsAsync(userRoles);
            postsRepositoryMock.Setup(r => r.GetPostsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(posts);
            mapperMock.Setup(m => m.Map<List<PostDto>>(posts)).Returns(postDtos.Where(p => !p.Hidden).ToList());

            // Act
            var result = await postService.GetAllPostsAsync(CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.Should().Contain(postDtos[0]);
        }

        [Fact]
        public async Task get_all_posts_should_throw_if_user_does_not_exist()
        {
            // Arrange
            var randomId = Guid.NewGuid();

            currentRequestAccessorMock.Setup(c => c.Id).Returns(randomId);

            // Act
            var result = async () => await postService.GetAllPostsAsync(CancellationToken.None);

            // Assert
            await result.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"User with Id: {randomId} was not found");
        }

        [Fact]
        public async Task get_post_by_id_should_return_post()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "test@test.com" };
            var userRoles = new List<string> { "Admin" };
            var post = new Post { Id = Guid.NewGuid(), Title = "Some Title" };
            var postDto = new PostDto { Id = post.Id, Title = post.Title };

            currentRequestAccessorMock.Setup(c => c.Id).Returns(user.Id);
            userRepositoryMock.Setup(s => s.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            userRepositoryMock.Setup(s => s.GetRolesForUserAsync(user)).ReturnsAsync(userRoles);
            postsRepositoryMock.Setup(r => r.GetPostByIdAsync(post.Id, It.IsAny<CancellationToken>())).ReturnsAsync(post);
            mapperMock.Setup(m => m.Map<PostDto>(post)).Returns(postDto);

            // Act
            var result = await postService.GetPostByIdAsync(post.Id, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(postDto);
        }

        [Fact]
        public async Task get_post_by_id_should_throw_if_user_was_not_found()
        {
            // Arrange
            var randomId = Guid.NewGuid();
            var post = new Post { Id = Guid.NewGuid() };

            currentRequestAccessorMock.Setup(c => c.Id).Returns(randomId);

            // Act
            var result = async () => await postService.GetPostByIdAsync(post.Id, CancellationToken.None);

            // Assert

            await result.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"User with Id: {randomId} was not found");
        }

        [Fact]
        public async Task get_post_by_id_should_throw_if_post_was_not_found()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "test@test.com" };
            var userRoles = new List<string> { "Admin" };
            var randomId = Guid.NewGuid();

            currentRequestAccessorMock.Setup(c => c.Id).Returns(user.Id);
            userRepositoryMock.Setup(s => s.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            userRepositoryMock.Setup(s => s.GetRolesForUserAsync(user)).ReturnsAsync(userRoles);

            // Act
            var result = async () => await postService.GetPostByIdAsync(randomId, CancellationToken.None);

            // Assert
            
            await result.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"Post with Id: {randomId} was not found");
        }

        [Fact]
        public async Task get_post_by_id_should_throw_if_unauthorized_to_view()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "test@test.com" };
            var userRoles = new List<string> { "User" };
            var post = new Post { Id = Guid.NewGuid(), Title = "Some Title", Hidden = true };

            currentRequestAccessorMock.Setup(c => c.Id).Returns(user.Id);
            userRepositoryMock.Setup(s => s.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            userRepositoryMock.Setup(s => s.GetRolesForUserAsync(user)).ReturnsAsync(userRoles);
            postsRepositoryMock.Setup(r => r.GetPostByIdAsync(post.Id, It.IsAny<CancellationToken>())).ReturnsAsync(post);

            // Act
            var result = async () => await postService.GetPostByIdAsync(post.Id, CancellationToken.None);

            // Assert
            await result.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("You are not authorized to view this post");
        }

        [Fact]
        public async Task create_post_should_return_added_post()
        {
            // Arrange
            var post = new Post { Id = Guid.NewGuid(), Title = "Some Title", Content = "Some Content", Hidden = true };
            var postDto = new PostDto { Id = post.Id, Title = post.Title, Content = post.Content, Hidden = post.Hidden };

            mapperMock.Setup(m => m.Map<Post>(postDto)).Returns(post);
            mapperMock.Setup(m => m.Map<PostDto>(post)).Returns(postDto);
            postsRepositoryMock.Setup(r => r.AddPostAsync(post, It.IsAny<CancellationToken>())).ReturnsAsync(post);
            postHubMock.Setup(p => p.Clients.All).Returns(postClientMock.Object);
            postClientMock.Setup(p => p.PostCreated(postDto)).Returns(Task.CompletedTask);

            // Act
            var result = await postService.CreatePostAsync(postDto, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(postDto);
            result.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            result.LastUpdatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            postsRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            postClientMock.Verify(p => p.PostCreated(postDto), Times.Once);
        }

        [Fact]
        public async Task create_post_should_throw_if_post_has_no_title_or_content()
        {
            // Arrange
            var post = new Post { Id = Guid.NewGuid() };
            var postDto = new PostDto { Id = post.Id };

            // Act
            var result = async () => await postService.CreatePostAsync(postDto, CancellationToken.None);

            // Assert
            await result.Should().ThrowAsync<ArgumentException>().WithMessage("Post title and content cannot be empty");
        }

        [Fact]
        public async Task update_post_should_update_and_return_post()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "test@test.com" };
            var userRoles = new List<string> { "Admin" };
            var post = new Post { Id = Guid.NewGuid(), Title = "Some Title", Content = "Some Content", Hidden = true };
            var updateRequest = new UpdatePostRequest
            {
                Title = "Updated Title",
                Content = "Updated Content",
                Hidden = false
            };
            var postDto = new PostDto { Id = post.Id, Title = updateRequest.Title, Content = updateRequest.Content,
                Hidden = updateRequest.Hidden, LastUpdatedDate = DateTime.UtcNow };

            currentRequestAccessorMock.Setup(c => c.Id).Returns(user.Id);
            userRepositoryMock.Setup(s => s.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            userRepositoryMock.Setup(s => s.GetRolesForUserAsync(user)).ReturnsAsync(userRoles);
            postsRepositoryMock.Setup(r => r.GetPostByIdAsync(post.Id, It.IsAny<CancellationToken>())).ReturnsAsync(post);
            mapperMock.Setup(m => m.Map<PostDto>(post)).Returns(postDto);
            postHubMock.Setup(p => p.Clients.All).Returns(postClientMock.Object);
            postClientMock.Setup(p => p.PostUpdated(postDto)).Returns(Task.CompletedTask);

            // Act
            var result = await postService.UpdatePostAsync(post.Id, updateRequest, CancellationToken.None);

            // Assert
            post.Title.Should().Be(updateRequest.Title);
            post.Content.Should().Be(updateRequest.Content);
            post.Hidden.Should().Be(updateRequest.Hidden);
            post.LastUpdatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(postDto);

            postsRepositoryMock.Verify(r => r.UpdatePostAsync(post), Times.Once);
            postsRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            postClientMock.Verify(p => p.PostUpdated(postDto), Times.Once);
        }

        [Fact]
        public async Task update_post_should_throw_if_user_was_not_found()
        {
            // Arrange
            var post = new Post { Id = Guid.NewGuid() };
            var userRoles = new List<string> { "Admin" };
            var randomId = Guid.NewGuid();
            var updateRequest = new UpdatePostRequest
            {
                Title = "Updated Title",
                Content = "Updated Content",
                Hidden = false
            };

            currentRequestAccessorMock.Setup(c => c.Id).Returns(randomId);

            // Act
            var result = async () => await postService.UpdatePostAsync(post.Id, updateRequest, CancellationToken.None);

            // Assert
            await result.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"User with Id: {randomId} was not found");
        }

        [Fact]
        public async Task update_post_should_throw_if_post_was_not_found()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "test@test.com" };
            var userRoles = new List<string> { "Admin" };
            var randomId = Guid.NewGuid();
            var updateRequest = new UpdatePostRequest
            {
                Title = "Updated Title",
                Content = "Updated Content",
                Hidden = false
            };

            currentRequestAccessorMock.Setup(c => c.Id).Returns(user.Id);
            userRepositoryMock.Setup(s => s.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            userRepositoryMock.Setup(s => s.GetRolesForUserAsync(user)).ReturnsAsync(userRoles);

            // Act
            var result = async () => await postService.UpdatePostAsync(randomId, updateRequest, CancellationToken.None);

            // Assert
            await result.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"Post with Id: {randomId} was not found");
        }

        [Fact]
        public async Task update_post_should_throw_if_user_is_unauthorized_to_update()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "test@test.com" };
            var userRoles = new List<string> { "User" };
            var post = new Post { Id = Guid.NewGuid(), Title = "Some Title", Content = "Some Content" };
            var updateRequest = new UpdatePostRequest
            {
                Title = "Updated Title",
                Content = "Updated Content",
                Hidden = false
            };

            currentRequestAccessorMock.Setup(c => c.Id).Returns(user.Id);
            userRepositoryMock.Setup(s => s.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            userRepositoryMock.Setup(s => s.GetRolesForUserAsync(user)).ReturnsAsync(userRoles);
            postsRepositoryMock.Setup(r => r.GetPostByIdAsync(post.Id, It.IsAny<CancellationToken>())).ReturnsAsync(post);

            // Act
            var result = async () => await postService.UpdatePostAsync(post.Id, updateRequest, CancellationToken.None);

            // Assert
            await result.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("You are not authorized to update this post");
        }

        [Fact]
        public async Task delete_post_by_id_should_delete_post()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "test@test.com" };
            var userRoles = new List<string> { "Admin" };
            var post = new Post { Id = Guid.NewGuid() };

            currentRequestAccessorMock.Setup(c => c.Id).Returns(user.Id);
            userRepositoryMock.Setup(s => s.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            userRepositoryMock.Setup(s => s.GetRolesForUserAsync(user)).ReturnsAsync(userRoles);
            postsRepositoryMock.Setup(r => r.GetPostByIdAsync(post.Id, It.IsAny<CancellationToken>())).ReturnsAsync(post);
            postHubMock.Setup(p => p.Clients.All).Returns(postClientMock.Object);
            postClientMock.Setup(p => p.PostDeleted(post.Id)).Returns(Task.CompletedTask);

            // Act
            await postService.DeletePostByIdAsync(post.Id, CancellationToken.None);

            // Assert
            postsRepositoryMock.Verify(r => r.DeletePostAsync(post), Times.Once);
            postsRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            postClientMock.Verify(p => p.PostDeleted(post.Id), Times.Once);
        }

        [Fact]
        public async Task delete_post_by_id_should_throw_if_user_was_not_found()
        {
            // Arrange
            var post = new Post { Id = Guid.NewGuid() };
            var randomId = Guid.NewGuid();

            currentRequestAccessorMock.Setup(c => c.Id).Returns(randomId);

            // Act
            var result = async () => await postService.DeletePostByIdAsync(post.Id, CancellationToken.None);

            // Assert
            await result.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"User with Id: {randomId} was not found");
        }

        [Fact]
        public async Task delete_post_by_id_should_throw_if_post_was_not_found()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "test@test.com" };
            var userRoles = new List<string> { "Admin" };
            var randomId = Guid.NewGuid();

            currentRequestAccessorMock.Setup(c => c.Id).Returns(user.Id);
            userRepositoryMock.Setup(s => s.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            userRepositoryMock.Setup(s => s.GetRolesForUserAsync(user)).ReturnsAsync(userRoles);

            // Act
            var result = async () => await postService.DeletePostByIdAsync(randomId, CancellationToken.None);

            // Assert
            await result.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"Post with Id: {randomId} was not found");
        }

        [Fact]
        public async Task delete_post_by_id_should_throw_if_user_is_unauthorized_to_delete_post()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "test@test.com" };
            var userRoles = new List<string> { "User" };
            var post = new Post { Id = Guid.NewGuid() };

            currentRequestAccessorMock.Setup(c => c.Id).Returns(user.Id);
            userRepositoryMock.Setup(s => s.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            userRepositoryMock.Setup(s => s.GetRolesForUserAsync(user)).ReturnsAsync(userRoles);
            postsRepositoryMock.Setup(r => r.GetPostByIdAsync(post.Id, It.IsAny<CancellationToken>())).ReturnsAsync(post);

            // Act
            var result = async () => await postService.DeletePostByIdAsync(post.Id, CancellationToken.None);

            // Assert
            await result.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("You are not authorized to delete this post");
        }
    }
}
