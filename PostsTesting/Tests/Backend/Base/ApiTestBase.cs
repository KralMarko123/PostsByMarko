using aspnetserver.Data.Models;
using aspnetserver.Data.Models.Dtos;
using aspnetserver.Data.Models.Responses;
using Newtonsoft.Json;
using RestSharp;
using Xunit;

namespace PostsTesting.Tests.Backend.Base
{
    public class ApiTestBase : BackendBase, IAsyncLifetime
    {
        public async Task InitializeAsync()
        {
            await base.InitializeAsync();
            await AddAuthenticationTokenToClient();
        }

        public async Task<string?> GetAuthenticationToken()
        {
            var request = new UserLoginDto { UserName = testUser.UserName, Password = "Test123" };
            var response = await client.PostJsonAsync<UserLoginDto, RequestResult>("/login", request);
            var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(response.Payload.ToString());

            return loginResponse?.Token;
        }

        public async Task AddAuthenticationTokenToClient()
        {
            var token = await GetAuthenticationToken();
            client.AddDefaultParameter(new HeaderParameter("Authorization", $"Bearer {token}"));
        }

        public async Task<T?> GetAsJson<T>(string url)
        {
            return await client.GetJsonAsync<T>(url);
        }

        public async Task<T?> PostAsJson<T>(string url, object payload = null)
        {
            return await client.PostJsonAsync<object, T>(url, payload);
        }

        public async Task<T?> Post<T>(string url)
        {
            var request = new RestRequest(url, Method.Post);
            return await client.PostAsync<T>(request);
        }

        public async Task<T?> PutAsJson<T>(string url, object payload)
        {
            return await client.PutJsonAsync<object, T>(url, payload);
        }

        public async Task<T?> DeleteAsJson<T>(string url)
        {
            var request = new RestRequest(url, Method.Delete);

            return await client.DeleteAsync<T>(request);
        }
    }
}
