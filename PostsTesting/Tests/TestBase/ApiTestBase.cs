using aspnetserver.Data.Models.Dtos;
using aspnetserver.Data.Models.Responses;
using RestSharp;
using Xunit;

namespace PostsTesting.Tests.TestBase
{
    public class ApiTestBase : Base, IAsyncLifetime
    {
        public async Task InitializeAsync() => await base.InitializeAsync();


        public async Task<string?> GetAuthenticationToken()
        {
            var request = new RestRequest()
            {

                Resource = "/login",
                Method = Method.Post,
            };
            request.AddBody(new UserLoginDto { UserName = testUser.UserName, Password = "Test123" }, "application/json");

            var response = await client.PostAsync<LoginResponse>(request);

            return response?.Token;
        }

        public async Task AddAuthenticationTokenToClient()
        {
            var token = await GetAuthenticationToken();
            client.AddDefaultParameter(new HeaderParameter("Authorization", $"Bearer {token}"));
        }

        public async Task<RestResponse> Get(string url)
        {
            await AddAuthenticationTokenToClient();

            var request = new RestRequest()
            {
                Resource = url,
                Method = Method.Get,
            };

            return await client.GetAsync(request);
        }

        public async Task<RestResponse> Post(string url, object? payload)
        {
            await AddAuthenticationTokenToClient();

            var request = new RestRequest()
            {
                Resource = url,
                Method = Method.Post,

            };
            request.AddHeader("Content-type", "application-json");
            request.AddBody(payload);

            return await client.PostAsync(request);
        }

        public async Task<RestResponse> Put(string url, object? payload)
        {
            await AddAuthenticationTokenToClient();

            var request = new RestRequest()
            {
                Resource = url,
                Method = Method.Put,

            };
            request.AddHeader("Content-type", "application-json");
            request.AddBody(payload);

            return await client.PutAsync(request);
        }

        public async Task<RestResponse> Delete(string url)
        {
            await AddAuthenticationTokenToClient();

            var request = new RestRequest()
            {
                Resource = url,
                Method = Method.Delete,
            };

            return await client.DeleteAsync(request);
        }
    }
}
