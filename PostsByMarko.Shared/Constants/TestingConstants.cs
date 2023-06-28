using PostsByMarko.Host.Constants;
using PostsByMarko.Host.Data.Models;

namespace PostsByMarko.Shared.Constants
{
    public static class TestingConstants
    {
        public static readonly User TEST_USER = AppConstants.DEFAULT_USERS.Last();
        public static string TEST_PASSWORD = "@PostsByMarko123";
    }
}