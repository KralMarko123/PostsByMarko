using aspnetserver.Data.Models;
using aspnetserver.Data.Models.Responses;
using PostsTesting.Utility.Constants;
using RestSharp;

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

        public async Task<List<Post>?> GetAllPosts()
        {
            return await GetAsJson<List<Post>>($"{BASE_URL}{GET_ALL_POSTS}");
        }

        public async Task<PostDetailsResponse?> GetPostById(string postId)
        {
            return await GetAsJson<PostDetailsResponse>($"{BASE_URL}{GET_POST}/{postId}");
        }

        public async Task<RestResponse?> CreatePost(object postToCreate)
        {
            return await Post($"{BASE_URL}{CREATE_POST}", postToCreate);
        }

        public async Task<RestResponse?> UpdatePost(object updatedPost)
        {
            return await Put($"{BASE_URL}{UPDATE_POST}", updatedPost);
        }

        public async Task<RestResponse?> DeletePostById(string postId)
        {
            return await Delete($"{BASE_URL}{DELETE_POST}/{postId}");
        }
    }
}
