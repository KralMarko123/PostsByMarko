using PostsByMarko.Host.Data.Models;

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

        public static int GetRandomNumberWithMax(int max)
        {
            return random.Next(max);
        }

        public static User GetRandomTestUser()
        {
            return new User($"test@{GetRandomTextWithLength(4)}.com", "Test", "User", true);
        }

        public static Post GetRandomPost()
        {
            return new Post($"Test Title: {GetRandomTextWithLength(5)}", $"Test Content: {GetRandomTextWithLength(20)}");
        }
    }
}
