using aspnetserver.Data.Models;
using PostsTesting.Utility;
using System.Text.Json;

namespace PostsTesting.Tests.Backend.Base
{
    public static class PostsDbTestBase
    {
        public static async Task<List<Post>?> GetAllPosts()
        {
            var query = "SELECT * FROM Posts";
            return await ExecuteSearchQuery(query);
        }

        public static async Task<Post?> GetPostById(int postId)
        {
            var query = $"SELECT * FROM Posts WHERE PostId = '{postId}'";
            var posts = await ExecuteSearchQuery(query);

            if (posts == null) throw new Exception($"Post with id: {postId} was not found");
            return posts.First();
        }

        public static async Task DeleteAllTestPosts()
        {
            var query = "DELETE FROM Posts WHERE Title LIKE '%Test%'";
            await ExecuteUpdatingQuery(query);
        }

        public static async Task<List<Post>?> ExecuteSearchQuery(string query)
        {
            var posts = new List<Post>();
            var reader = await DbUtility.ExecuteQueryAsync(query);

            while (reader.Read())
            {
                var post = new Post()
                {
                    PostId = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Content = reader.GetString(2),
                    UserId = reader.GetString(3),
                    CreatedDate = reader.GetDateTime(4),
                    LastUpdatedDate = reader.GetDateTime(5),
                    IsHidden = reader.GetBoolean(6),
                    AllowedUsers = JsonSerializer.Deserialize<List<string>>(reader.GetString(7))
                };

                posts.Add(post);
            }

            await DbUtility.CloseConnectionAsync();
            return posts;
        }

        public static async Task ExecuteUpdatingQuery(string query)
        {
            var result = await DbUtility.ExecuteUpdateQueryAsync(query);
            // Check for error during insertion.
            if (result < 0) throw new Exception("Error during database modification");

            await DbUtility.CloseConnectionAsync();
        }
    }
}
