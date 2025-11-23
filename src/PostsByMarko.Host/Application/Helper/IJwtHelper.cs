using PostsByMarko.Host.Data.Entities;

namespace PostsByMarko.Host.Application.Helper
{
    public interface IJwtHelper
    {
        Task<string> CreateTokenAsync(User user);
    }
}
