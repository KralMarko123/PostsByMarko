using aspnetserver.Data.Models;
using aspnetserver.Data.Models.Responses;
using PostsTesting.Utility;
using PostsTesting.Utility.Constants;

namespace PostsTesting.Tests.TestBase
{
    public class PostsApiTestBase : ApiTestBase
    {
        private static string BASE_URL = TestingConstants.ServerEndpoint;
        private const string GET_ALL_POSTS = "/get-all-posts";
        private const string GET_POST = "/get-post-by-id";
        private const string CREATE_POST = "/create-post";
        private const string UPDATE_POST = "/update-post";
        private const string DELETE_POST = "/delete-post-by-id";

        public async Task<List<Post>> GetAllPosts()
        {
            var response = await Get($"{BASE_URL}{GET_ALL_POSTS}");
            return SimpleJsonSerializer.Deserialize<List<Post>>(response);
        }

        public async Task<PostDetailsResponse> GetPostById(string postId)
        {
            var response = await Get($"{BASE_URL}{GET_POST}/{postId}");
            return SimpleJsonSerializer.Deserialize<PostDetailsResponse>(response);
        }

        public async Task<string> CreatePost(object? postToCreate)
        {
            var response = await Post($"{BASE_URL}{CREATE_POST}", postToCreate);
            return SimpleJsonSerializer.Deserialize<string>(response);
        }

        public async Task<string> UpdatePost(object? updatedPost)
        {
            var response = await Put($"{BASE_URL}{UPDATE_POST}", updatedPost);
            return SimpleJsonSerializer.Deserialize<string>(response);
        }

        public async Task<string> DeletePostById(string postId)
        {
            var response = await Delete($"{BASE_URL}{DELETE_POST}/{postId}");
            return SimpleJsonSerializer.Deserialize<string>(response);
        }
    }
}
