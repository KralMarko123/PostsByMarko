using aspnetserver.Data.Models;

namespace aspnetserver.Helper
{
    public interface IJwtHelper
    {
        Task<string> CreateTokenAsync(User user);
    }
}
