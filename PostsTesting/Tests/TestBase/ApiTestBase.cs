using aspnetserver.Data.Models.Dtos;
using aspnetserver.Data.Models.Responses;
using RestSharp;
using Xunit;

namespace PostsTesting.Tests.TestBase
{
    public class ApiTestBase : Base, IAsyncLifetime
    {
        public async Task InitializeAsync()
        {
            await base.InitializeAsync();
            await AddAuthenticationTokenToClient();
        }

        public async Task<string?> GetAuthenticationToken()
        {
            var request = new UserLoginDto { UserName = testUser.UserName, Password = "Test123" };
            var response = await client.PostJsonAsync<UserLoginDto, LoginResponse>("/login", request);

            return response?.Token;
        }

        public async Task AddAuthenticationTokenToClient()
        {
            var token = await GetAuthenticationToken();
            client.AddDefaultParameter(new HeaderParameter("Authorization", $"Bearer {token}"));
        }

        public async Task<RestResponse> Get(string url)
        {
            var request = new RestRequest(url, Method.Get);

            return await client.GetAsync(request);
        }

        public async Task<T?> GetAsJson<T>(string url)
        {
            return await client.GetJsonAsync<T>(url);
        }

        public async Task<RestResponse> Post(string url, object payload)
        {
            var request = new RestRequest(url, Method.Post);
            request.AddBody(payload);

            return await client.PostAsync(request);
        }

        public async Task<T?> PostAsJson<T>(string url, object payload)
        {
            return await client.PostJsonAsync<object, T>(url, payload);
        }

        public async Task<RestResponse> Put(string url, object? payload)
        {
            var request = new RestRequest(url, Method.Put);
            request.AddBody(payload);

            return await client.PutAsync(request);
        }

        public async Task<T?> PutAsJson<T>(string url, object payload)
        {
            return await client.PutJsonAsync<object, T>(url, payload);
        }

        public async Task<RestResponse> Delete(string url)
        {
            var request = new RestRequest(url, Method.Delete);

            return await client.DeleteAsync(request);
        }
    }
}
