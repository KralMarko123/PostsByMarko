using PostsByMarko.Host.Application.Requests;
using PostsByMarko.Host.Application.Responses;

namespace PostsByMarko.Host.Application.Interfaces
{
    public interface IAdminService
    {
        Task<List<AdminDashboardResponse>> GetAdminDashboardAsync(CancellationToken cancellationToken = default);
        Task<List<string>> GetRolesForEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<List<string>> UpdateUserRolesAsync(UpdateUserRolesRequest request, CancellationToken cancellationToken = default);
        Task DeleteUserByIdAsync(Guid Id, CancellationToken cancellationToken = default);
    }
}
