namespace aspnetserver.Helper
{
    public interface IJwtHelper
    {
        Task<string> CreateTokenAsync();
    }
}
