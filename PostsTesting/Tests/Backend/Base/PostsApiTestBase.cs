using aspnetserver.Data.Models;
using PostsTesting.Utility.Constants;

namespace PostsTesting.Tests.Backend.Base
{
    public class PostsApiTestBase : ApiTestBase
    {
        private static string BASE_URL = TestingConstants.ServerEndpoint;
        private const string GET_ALL_POSTS = "/get-all-posts";
        private const string GET_POST = "/get-post-by-id";
        private const string CREATE_POST = "/create-post";
        private const string UPDATE_POST = "/update-post";
        private const string DELETE_POST = "/delete-post-by-id";
        private const string TOGGLE_POST_HIDDEN = "/toggle-post-visibility";


        public async Task<RequestResult?> GetAllPosts()
        {
            return await GetAsJson<RequestResult>($"{BASE_URL}{GET_ALL_POSTS}");
        }

        public async Task<RequestResult?> GetPostById(string postId)
        {
            return await GetAsJson<RequestResult>($"{BASE_URL}{GET_POST}/{postId}");
        }

        public async Task<RequestResult?> GetPostByIdResponse(string postId)
        {
            return await GetAsJson<RequestResult>($"{BASE_URL}{GET_POST}/{postId}");
        }

        public async Task<RequestResult?> CreatePost(object postToCreate)
        {
            return await PostAsJson<RequestResult>($"{BASE_URL}{CREATE_POST}", postToCreate);
        }

        public async Task<RequestResult?> UpdatePost(object updatedPost)
        {
            return await PutAsJson<RequestResult>($"{BASE_URL}{UPDATE_POST}", updatedPost);
        }

        public async Task<RequestResult?> DeletePostById(string postId)
        {
            return await DeleteAsJson<RequestResult>($"{BASE_URL}{DELETE_POST}/{postId}");
        }
        public async Task<RequestResult?> TogglePostVisibilityById(string postId)
        {
            return await Post<RequestResult>($"{BASE_URL}{TOGGLE_POST_HIDDEN}/{postId}");
        }
    }
}
