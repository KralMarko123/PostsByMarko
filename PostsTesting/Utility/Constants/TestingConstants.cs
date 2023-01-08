using aspnetserver.Constants;
using aspnetserver.Data.Models;

namespace PostsTesting.Utility.Constants
{
    public static class TestingConstants
    {
        private static readonly string environment = ConfigReader.GetConfigurationEntries().FirstOrDefault(e => e.Key.Equals("Environment")).Value;

        private const string baseUrlDev = "https://localhost:7171";
        private const string baseUiUrlDev = "http://localhost:3000";
        private const string baseUrlPrd = "https://posts-aspnetserver.azurewebsites.net";
        private const string baseUiUrlPrd = "https://posts.markomarkovikj.com";
        public const string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=PostsDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public static readonly string browserType = ConfigReader.GetConfigurationEntries().FirstOrDefault(e => e.Key.Equals("Browser")).Value;
        public static string ServerEndpoint
        {
            get
            {
                switch (environment)
                {
                    case "DEV":
                        return baseUrlDev;

                    case "PRD":
                        return baseUrlPrd;

                    default:
                        return string.Empty;
                }
            }
        }

        public static string UiEndpoint
        {
            get
            {
                switch (environment)
                {
                    case "DEV":
                        return baseUiUrlDev;

                    case "PRD":
                        return baseUiUrlPrd;

                    default:
                        return string.Empty;
                }
            }
        }

        public static readonly User AdminUser = AppConstants.appUsers.First();

        public static readonly User TestUser = AppConstants.appUsers.Last();
    }
}