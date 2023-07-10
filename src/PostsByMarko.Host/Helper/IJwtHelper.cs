using PostsByMarko.Host.Data.Models;

namespace PostsByMarko.Host.Helper
{
    public interface IJwtHelper
    {
        Task<string> CreateTokenAsync(User user);
    }
}
