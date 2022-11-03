

namespace PostsTesting.Utility
{
    public class RandomDataGenerator
    {
        private static string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static Random random = new Random();

        public static string GetRandomTextWithLength(int length)
        {
            return new string(Enumerable.Repeat(characters, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}
