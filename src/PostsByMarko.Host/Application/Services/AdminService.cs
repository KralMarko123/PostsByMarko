using Microsoft.AspNetCore.SignalR;
using PostsByMarko.Host.Application.Enums;
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
            var user = await userRepository.GetUserByIdAsync(request.UserId!.Value, cancellationToken) ?? throw new KeyNotFoundException($"User with Id: {request.UserId} was not found");
            var currentRoles = await userRepository.GetRolesForUserAsync(user);

            if (request.ActionType == ActionType.Create)
            {
                if (currentRoles.Contains(request.Role)) return [.. currentRoles];

                await userRepository.AddRoleToUserAsync(user, request.Role);
            }
            else if (request.ActionType == ActionType.Delete)
            {
                if (!currentRoles.Contains(request.Role)) return [.. currentRoles];

                await userRepository.RemoveRoleFromUserAsync(user, request.Role);
            }

            var updatedRoles = await userRepository.GetRolesForUserAsync(user);

            await adminHub.Clients.All.UpdatedUserRoles(user.Id, DateTime.UtcNow);

            return [.. updatedRoles];
        }

        public async Task<List<string>> GetRolesForEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.GetUserByEmailAsync(email, cancellationToken) ?? throw new KeyNotFoundException($"User with email: '{email}' was not found");
            var roles = await userRepository.GetRolesForUserAsync(user);

            return [.. roles];
        }

        public async Task DeleteUserByIdAsync(Guid Id, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.GetUserByIdAsync(Id, cancellationToken) ?? throw new KeyNotFoundException($"User with Id: {Id} was not found");
            var result = await userRepository.DeleteUserAsync(user);

            if (result.Succeeded) 
            {
                await adminHub.Clients.All.DeletedUser(user.Id, DateTime.UtcNow);    
            }
            else
            {
                throw new InvalidOperationException($"Failed to delete user with Id: {user.Id}");
            }
        }
    }
}
