using PostsByMarko.Host.Constants;
using PostsByMarko.Host.Data.Models;

namespace PostsByMarko.Shared.Constants
{
    public static class TestingConstants
    {
        public static readonly User TEST_USER = AppConstants.DEFAULT_USERS.Last();
        public const string TEST_PASSWORD = "@PostsByMarko123";
        public const string DEVELOPMENT_HOST_ENDPOINT = "http://localhost:7171";
        public const string DEV_CLIENT_ENDPOINT = "http://localhost:3000";
        public const string FIREFOX_BROWSER = "Firefox";
        public const string CHROME_BROWSER = "Chrome";
        public const bool HEADLESS_BROWSER = false;
    }
}