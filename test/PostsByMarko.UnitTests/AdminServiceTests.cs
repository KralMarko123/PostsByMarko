using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Moq;
using PostsByMarko.Host.Application.Constants;
using PostsByMarko.Host.Application.Enums;
using PostsByMarko.Host.Application.Exceptions;
using PostsByMarko.Host.Application.Hubs;
using PostsByMarko.Host.Application.Hubs.Client;
using PostsByMarko.Host.Application.Interfaces;
using PostsByMarko.Host.Application.Requests;
using PostsByMarko.Host.Application.Services;
using PostsByMarko.Host.Data.Entities;
using PostsByMarko.Host.Data.Repositories.Users;

namespace PostsByMarko.UnitTests
{
    public class AdminServiceTests
    {
        private readonly AdminService adminService;
        private readonly Mock<IUserRepository> usersRepositoryMock = new();
        private readonly Mock<ICurrentRequestAccessor> currentRequestAccessorMock = new();
        private readonly Mock<IHubContext<AdminHub, IAdminClient>> adminHubMock = new();
        private readonly Mock<IAdminClient> adminClientMock = new();

        public AdminServiceTests()
        {
            usersRepositoryMock
                .Setup(r => r.GetRolesForUserAsync(It.IsAny<User>()))
                .ReturnsAsync([]);
            adminService = new AdminService(usersRepositoryMock.Object, currentRequestAccessorMock.Object, adminHubMock.Object);
        }

        [Fact]
        public async Task get_admin_dashboard_should_return_dashboard_data()
        {
            // Arrange
            var admin = new User() { Id = Guid.NewGuid() };
            var today = DateTime.Now;
            var users = new List<User>()
            {
                new User { Id = Guid.NewGuid() },
                new User
                {
                    Id = Guid.NewGuid(),
                    Email = "test@test.com",
                    Posts = new List<Post>
                    {
                        new Post { Id = Guid.NewGuid(), LastUpdatedAt = today },
                        new Post { Id = Guid.NewGuid() }
                    }
                }
            };
            var userRoles = new List<string> { "Some role" };

            currentRequestAccessorMock.Setup(a => a.Id).Returns(admin.Id);
            usersRepositoryMock.Setup(r => r.GetUsersAsync(admin.Id, It.IsAny<CancellationToken>())).ReturnsAsync(() => users);
            usersRepositoryMock.Setup(r => r.GetRolesForUserAsync(It.IsAny<User>())).ReturnsAsync(() => userRoles);

            // Act
            var result = await adminService.GetAdminDashboardAsync(CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(users.Count);

            var userWithData = result[1];

            userWithData.UserId.Should().Be(users[1].Id);
            userWithData.Email.Should().Be(users[1].Email);
            userWithData.NumberOfPosts.Should().Be(2);
            userWithData.LastPostedAt.Should().BeWithin(TimeSpan.FromSeconds(5));
            userWithData.Roles.Should().BeEquivalentTo(userRoles);
        }

        [Fact]
        public async Task get_roles_for_email_should_return_user_roles()
        {
            // Arrange
            var user = new User() { Id = Guid.NewGuid(), Email = "test@test.com" };
            var roles = new List<string> { "Role1", "Role2" };

            usersRepositoryMock.Setup(r => r.GetUserByEmailAsync(user.Email, It.IsAny<CancellationToken>())).ReturnsAsync(() => user);
            usersRepositoryMock.Setup(r => r.GetRolesForUserAsync(user)).ReturnsAsync(() => roles);

            // Act
            var result = await adminService.GetRolesForEmailAsync(user.Email);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(roles);
        }

        [Fact]
        public async Task update_user_roles_should_return_with_new_role()
        {
            // Arrange
            var user = new User() { Id = Guid.NewGuid() };
            var request = new UpdateUserRolesRequest
            {
                UserId = user.Id,
                ActionType = ActionType.Create,
                Role = RoleConstants.USER
            };
            var currentRoles = new List<string> { "Some role" };
            var updatedRoles = new List<string> { currentRoles[0], request.Role };

            usersRepositoryMock.Setup(r => r.GetUserByIdAsync(request.UserId.Value, It.IsAny<CancellationToken>())).ReturnsAsync(() => user);
            usersRepositoryMock.Setup(r => r.AddRoleToUserAsync(user, request.Role)).ReturnsAsync(IdentityResult.Success);
            usersRepositoryMock.Setup(r => r.GetRolesForUserAsync(user)).ReturnsAsync(() => updatedRoles);
            adminHubMock.Setup(a => a.Clients.All).Returns(adminClientMock.Object);
            adminClientMock.Setup(a => a.UpdatedUserRoles(user.Id, It.IsAny<DateTime>())).Returns(Task.CompletedTask);

            // Act
            var result = await adminService.UpdateUserRolesAsync(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(updatedRoles);
            usersRepositoryMock.Verify(r => r.AddRoleToUserAsync(user, request.Role), Times.Once);
            adminClientMock.Verify(a => a.UpdatedUserRoles(user.Id, It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task update_user_roles_should_return_with_removed_role()
        {
            // Arrange
            var user = new User() { Id = Guid.NewGuid() };
            var request = new UpdateUserRolesRequest
            {
                UserId = user.Id,
                ActionType = ActionType.Delete,
                Role = RoleConstants.USER
            };
            var currentRoles = new List<string> { "Some role", request.Role };
            var updatedRoles = new List<string> { currentRoles[0] };

            usersRepositoryMock.Setup(r => r.GetUserByIdAsync(request.UserId.Value, It.IsAny<CancellationToken>())).ReturnsAsync(() => user);
            usersRepositoryMock.Setup(r => r.RemoveRoleFromUserAsync(user, request.Role)).ReturnsAsync(IdentityResult.Success);
            usersRepositoryMock.SetupSequence(r => r.GetRolesForUserAsync(user)).ReturnsAsync(() => updatedRoles);
            adminHubMock.Setup(a => a.Clients.All).Returns(adminClientMock.Object);
            adminClientMock.Setup(a => a.UpdatedUserRoles(user.Id, It.IsAny<DateTime>())).Returns(Task.CompletedTask);

            // Act
            var result = await adminService.UpdateUserRolesAsync(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(updatedRoles);
            usersRepositoryMock.Verify(r => r.RemoveRoleFromUserAsync(user, request.Role), Times.Once);
            adminClientMock.Verify(a => a.UpdatedUserRoles(user.Id, It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task update_user_roles_should_throw_if_role_update_was_unsuccessful()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid() };
            var request = new UpdateUserRolesRequest
            {
                UserId = user.Id,
                ActionType = ActionType.Create,
                Role = RoleConstants.USER
            };

            usersRepositoryMock.Setup(r => r.GetUserByIdAsync(request.UserId.Value, It.IsAny<CancellationToken>())).ReturnsAsync(() => user);
            usersRepositoryMock.Setup(r => r.AddRoleToUserAsync(user, request.Role)).ReturnsAsync(IdentityResult.Failed());

            // Act
            var result = async () => await adminService.UpdateUserRolesAsync(request, CancellationToken.None);

            // Assert
            await result.Should().ThrowAsync<InvalidOperationException>().WithMessage($"Error while updating roles for user with Id: {user.Id}");
        }

        [Fact]
        public async Task update_user_roles_should_throw_if_user_was_not_found()
        {
            // Arrange
            var request = new UpdateUserRolesRequest
            {
                UserId = Guid.NewGuid(),
                ActionType = ActionType.Create,
                Role = "Admin",
            };

            // Act
            var result = async () => await adminService.UpdateUserRolesAsync(request, CancellationToken.None);

            // Assert
            await result.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"User with Id: {request.UserId} was not found");
        }

        [Fact]
        public async Task update_user_roles_should_reject_unknown_role()
        {
            var request = new UpdateUserRolesRequest
            {
                UserId = Guid.NewGuid(),
                ActionType = ActionType.Create,
                Role = "SuperAdmin"
            };

            var result = async () => await adminService.UpdateUserRolesAsync(request, CancellationToken.None);

            await result.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Role 'SuperAdmin' is not supported.");
            usersRepositoryMock.Verify(
                repository => repository.GetUserByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task update_user_roles_should_reject_removing_own_admin_role()
        {
            var adminId = Guid.NewGuid();
            currentRequestAccessorMock.Setup(accessor => accessor.Id).Returns(adminId);
            var request = new UpdateUserRolesRequest
            {
                UserId = adminId,
                ActionType = ActionType.Delete,
                Role = RoleConstants.ADMIN
            };

            var result = async () => await adminService.UpdateUserRolesAsync(request, CancellationToken.None);

            await result.Should().ThrowAsync<ArgumentException>()
                .WithMessage("You cannot remove your own administrator role.");
        }

        [Fact]
        public async Task update_user_roles_should_preserve_last_admin()
        {
            var user = new User { Id = Guid.NewGuid() };
            var request = new UpdateUserRolesRequest
            {
                UserId = user.Id,
                ActionType = ActionType.Delete,
                Role = RoleConstants.ADMIN
            };
            var protectedResult = IdentityResult.Failed(new IdentityError
            {
                Code = IdentityErrorCodes.LastMemberInRole
            });
            usersRepositoryMock
                .Setup(repository => repository.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            usersRepositoryMock
                .Setup(repository => repository.RemoveRoleFromUserUnlessLastMemberAsync(
                    user, RoleConstants.ADMIN, It.IsAny<CancellationToken>()))
                .ReturnsAsync(protectedResult);

            var result = async () => await adminService.UpdateUserRolesAsync(request, CancellationToken.None);

            await result.Should().ThrowAsync<ConflictException>()
                .WithMessage("At least one administrator account must remain.");
        }


        [Fact]
        public async Task delete_user_should_call_delete_method_in_repository()
        {
            // Arrange
            var user = new User() { Id = Guid.NewGuid() };

            usersRepositoryMock.Setup(r => r.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(() => user);
            usersRepositoryMock.Setup(r => r.DeleteUserAsync(user)).ReturnsAsync(IdentityResult.Success);
            adminHubMock.Setup(a => a.Clients.All).Returns(adminClientMock.Object);
            adminClientMock.Setup(a => a.DeletedUser(user.Id, It.IsAny<DateTime>())).Returns(Task.CompletedTask);

            // Act
            await adminService.DeleteUserByIdAsync(user.Id, CancellationToken.None);

            // Assert
            usersRepositoryMock.Verify(r => r.DeleteUserAsync(user), Times.Once);
            adminClientMock.Verify(a => a.DeletedUser(user.Id, It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task delete_user_should_throw_if_user_was_not_found()
        {
            // Arrange
            var randomId = Guid.NewGuid();

            // Act
            var result = async () => await adminService.DeleteUserByIdAsync(randomId, CancellationToken.None);

            // Assert
            await result.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"User with Id: {randomId} was not found");
        }

        [Fact]
        public async Task delete_user_should_throw_if_user_was_not_deleted()
        {
            // Arrange
            var user = new User() { Id = Guid.NewGuid() };
            var failedResult = IdentityResult.Failed();

            usersRepositoryMock.Setup(r => r.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(() => user);
            usersRepositoryMock.Setup(r => r.DeleteUserAsync(user)).ReturnsAsync(() => failedResult);

            // Act
            var result = async () => await adminService.DeleteUserByIdAsync(user.Id, CancellationToken.None);

            // Assert
            await result.Should().ThrowAsync<InvalidOperationException>().WithMessage($"Failed to delete user with Id: {user.Id}");
        }

        [Fact]
        public async Task delete_user_should_reject_deleting_own_account()
        {
            var adminId = Guid.NewGuid();
            currentRequestAccessorMock.Setup(accessor => accessor.Id).Returns(adminId);

            var result = async () => await adminService.DeleteUserByIdAsync(adminId, CancellationToken.None);

            await result.Should().ThrowAsync<ArgumentException>()
                .WithMessage("You cannot delete your own administrator account.");
            usersRepositoryMock.Verify(repository => repository.DeleteUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task delete_user_should_preserve_last_admin()
        {
            var admin = new User { Id = Guid.NewGuid() };
            var protectedResult = IdentityResult.Failed(new IdentityError
            {
                Code = IdentityErrorCodes.LastMemberInRole
            });
            usersRepositoryMock
                .Setup(repository => repository.GetUserByIdAsync(admin.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(admin);
            usersRepositoryMock
                .Setup(repository => repository.GetRolesForUserAsync(admin))
                .ReturnsAsync([RoleConstants.ADMIN]);
            usersRepositoryMock
                .Setup(repository => repository.DeleteUserUnlessLastMemberInRoleAsync(
                    admin, RoleConstants.ADMIN, It.IsAny<CancellationToken>()))
                .ReturnsAsync(protectedResult);

            var result = async () => await adminService.DeleteUserByIdAsync(admin.Id, CancellationToken.None);

            await result.Should().ThrowAsync<ConflictException>()
                .WithMessage("At least one administrator account must remain.");
        }
    }
}
