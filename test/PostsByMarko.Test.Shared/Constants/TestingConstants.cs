using PostsByMarko.Host.Constants;
using PostsByMarko.Host.Data.Models;

namespace PostsByMarko.Test.Shared.Constants
{
    public static class TestingConstants
    {
        public static readonly User MARKO = AppConstants.ADMINS[0];
        public static readonly User TEST_ADMIN = AppConstants.ADMINS[1];
        public static readonly User TEST_USER = AppConstants.USERS[0];
        public static readonly User RANDOM_USER = AppConstants.USERS[1];
        public const string TEST_PASSWORD = "@Marko123";
        public const string DEVELOPMENT_HOST_ENDPOINT = "http://localhost:7171";
        public const string DEV_CLIENT_ENDPOINT = "http://localhost:3000";
        public const string FIREFOX_BROWSER = "Firefox";
        public const string CHROME_BROWSER = "Chrome";
        public const bool HEADLESS_BROWSER = true;
        public const int UI_TIMEOUT_IN_MILLISECONDS = 10000;
    }
}