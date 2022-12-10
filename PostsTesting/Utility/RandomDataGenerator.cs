using aspnetserver.Data.Models;

namespace PostsTesting.Utility
{
    public class RandomDataGenerator
    {
        private const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static readonly Random random = new Random();

        public static string GetRandomTextWithLength(int length)
        {
            return new string(Enumerable.Repeat(characters, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static User GetRandomTestUser()
        {
            return new User
            {
                FirstName = $"Random {GetRandomTextWithLength(5)}",
                LastName = $"Randomson {GetRandomTextWithLength(5)}",
                UserName = $"test@{GetRandomTextWithLength(4)}.com",
                NormalizedUserName = $"test@{GetRandomTextWithLength(4)}.com".ToUpper(),
                Email = $"test@{GetRandomTextWithLength(4)}.com",
                NormalizedEmail = $"test@{GetRandomTextWithLength(4)}.com".ToUpper(),
                EmailConfirmed = true,
                Posts = new List<Post>(),
            };
        }
    }
}
