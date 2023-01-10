using aspnetserver.Constants;
using aspnetserver.Data.Models;

namespace PostsTesting.Utility.Constants
{
    public static class TestingConstants
    {
        public static readonly string serverEndpoint = ConfigReader.GetConfigurationEntries().FirstOrDefault(e => e.Key.Equals("ServerEndpoint")).Value;
        public static readonly string clientEndpoint = ConfigReader.GetConfigurationEntries().FirstOrDefault(e => e.Key.Equals("ClientEndpoint")).Value;
        public static readonly string browserType = ConfigReader.GetConfigurationEntries().FirstOrDefault(e => e.Key.Equals("Browser")).Value;
        public static readonly string connectionString = ConfigReader.GetConfigurationEntries().FirstOrDefault(e => e.Key.Equals("ConnectionString")).Value;

        public static readonly User AdminUser = AppConstants.appUsers.First();

        public static readonly User TestUser = AppConstants.appUsers.Last();
    }
}