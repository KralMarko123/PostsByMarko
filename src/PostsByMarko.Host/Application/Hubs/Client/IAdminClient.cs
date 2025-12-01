namespace PostsByMarko.Host.Application.Hubs.Client
{
    public interface IAdminClient
    {
        Task UpdatedUserRoles(Guid userId, DateTime timestamp);
        Task DeletedUser(Guid userId, DateTime timestamp);
    }
}
