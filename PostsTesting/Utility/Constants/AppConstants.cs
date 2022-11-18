using PostsTesting.Utility.Models;

namespace PostsTesting.Utility.Constants
{
    public static class AppConstants
    {
        private static readonly string environment = ConfigReader.GetConfigurationEntries().FirstOrDefault(e => e.Key.Equals("Environment")).Value;

        private const string baseUrlDev = "localhost:7171";
        private const string baseUiUrlDev = "localhost:3000";
        private const string baseUrlPrd = "https://posts-aspnetserver.azurewebsites.net";
        private const string baseUiUrlPrd = "https://posts.markomarkovikj.com";

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

        public static User TestUser = new User
        {
            FirstName = "Test",
            LastName = "Testerson",
            Username = "test@test.com",
            Password = "Test123"
        };
    }
}