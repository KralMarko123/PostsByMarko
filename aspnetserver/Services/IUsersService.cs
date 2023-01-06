using aspnetserver.Data.Models;

namespace aspnetserver.Services
{
    public interface IUsersService
    {
        Task<RequestResult> GetAllUsernamesAsync();
    }
}
