using PostsByMarko.Host.Application.Helper;
﻿using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Identity;
using PostsByMarko.Host.Application.Constants;
using PostsByMarko.Host.Application.Enums;
using PostsByMarko.Host.Application.Exceptions;
using PostsByMarko.Host.Application.Hubs;
using PostsByMarko.Host.Application.Hubs.Client;
using PostsByMarko.Host.Application.Interfaces;
using PostsByMarko.Host.Application.Requests;
using PostsByMarko.Host.Application.Responses;
using PostsByMarko.Host.Data.Repositories.Users;

namespace PostsByMarko.Host.Application.Services
{
    public class AdminService : IAdminService
    {
        private static readonly Dictionary<string, string> allowedRoles = new(StringComparer.OrdinalIgnoreCase)
        {
            [RoleConstants.ADMIN] = RoleConstants.ADMIN,
            [RoleConstants.USER] = RoleConstants.USER
        };

        private readonly IUserRepository userRepository;
        private readonly ICurrentRequestAccessor currentRequestAccessor;
        private readonly IHubContext<AdminHub, IAdminClient> adminHub;

        public AdminService(IUserRepository userRepository, ICurrentRequestAccessor currentRequestAccessor, IHubContext<AdminHub, IAdminClient> adminHub)
        {
            this.userRepository = userRepository;
            this.currentRequestAccessor = currentRequestAccessor;
            this.adminHub = adminHub;
        }

        public async Task<List<AdminDashboardResponse>> GetAdminDashboardAsync(CancellationToken cancellationToken = default)
        {
            var adminId = currentRequestAccessor.Id;
            var users = await userRepository.GetUsersAsync(adminId, cancellationToken);
            var result = new List<AdminDashboardResponse>();

            foreach (var user in users)
            {
                var roles = await userRepository.GetRolesForUserAsync(user);

                result.Add(new AdminDashboardResponse
                {
                    UserId = user.Id,
                    Email = user.Email!,
                    NumberOfPosts = user.Posts.Count,
                    LastPostedAt = user.Posts.MaxBy(p => p.LastUpdatedAt)?.LastUpdatedAt,
                    Roles = [.. roles]
                });
            }

            return result;
        }

        public async Task<List<string>> UpdateUserRolesAsync(UpdateUserRolesRequest request, CancellationToken cancellationToken = default)
        {
            if (request.UserId is null || request.UserId == Guid.Empty ||
                request.ActionType is not (ActionType.Create or ActionType.Delete) ||
                string.IsNullOrWhiteSpace(request.Role))
                throw new BadRequestException("A user, role, and create or delete action are required.");

            if (!allowedRoles.TryGetValue(request.Role.Trim(), out var role))
                throw new BadRequestException($"Role '{request.Role}' is not supported.");

            if (request.ActionType == ActionType.Delete &&
                role == RoleConstants.ADMIN &&
                request.UserId.Value == currentRequestAccessor.Id)
                throw new BadRequestException("You cannot remove your own administrator role.");

            var user = await userRepository.GetUserByIdAsync(request.UserId.Value, cancellationToken) ?? throw new ResourceNotFoundException($"User with Id: {request.UserId} was not found");
            var result = request.ActionType == ActionType.Create
                ? await userRepository.AddRoleToUserAsync(user, role)
                : role == RoleConstants.ADMIN
                    ? await userRepository.RemoveRoleFromUserUnlessLastMemberAsync(user, role, cancellationToken)
                    : await userRepository.RemoveRoleFromUserAsync(user, role);

            EnsureIdentityOperationSucceeded(result,
                $"Error while updating roles for user with Id: {request.UserId}");

            var updatedRoles = await userRepository.GetRolesForUserAsync(user);

            await NotificationDelivery.SendAsync(() => adminHub.Clients.All.UpdatedUserRoles(user.Id, DateTime.UtcNow));

            return [.. updatedRoles];
        }

        public async Task<List<string>> GetRolesForEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.GetUserByEmailAsync(email, cancellationToken) ?? throw new ResourceNotFoundException($"User with email: '{email}' was not found");
            var roles = await userRepository.GetRolesForUserAsync(user);

            return [.. roles];
        }

        public async Task DeleteUserByIdAsync(Guid Id, CancellationToken cancellationToken = default)
        {
            if (Id == currentRequestAccessor.Id)
                throw new BadRequestException("You cannot delete your own administrator account.");

            var user = await userRepository.GetUserByIdAsync(Id, cancellationToken) ?? throw new ResourceNotFoundException($"User with Id: {Id} was not found");
            var roles = await userRepository.GetRolesForUserAsync(user);
            var result = roles.Contains(RoleConstants.ADMIN, StringComparer.OrdinalIgnoreCase)
                ? await userRepository.DeleteUserUnlessLastMemberInRoleAsync(user, RoleConstants.ADMIN, cancellationToken)
                : await userRepository.DeleteUserAsync(user);

            EnsureIdentityOperationSucceeded(result, $"Failed to delete user with Id: {user.Id}");
            await NotificationDelivery.SendAsync(() => adminHub.Clients.All.DeletedUser(user.Id, DateTime.UtcNow));
        }

        private static void EnsureIdentityOperationSucceeded(IdentityResult result, string failureMessage)
        {
            if (result.Succeeded) return;

            if (result.Errors.Any(error => error.Code == IdentityErrorCodes.LastMemberInRole))
                throw new ConflictException("At least one administrator account must remain.");

            throw new InvalidOperationException(failureMessage);
        }
    }
}
