using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Host.Data.Models.Dtos;
using PostsByMarko.Host.Data.Models.Responses;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace PostsByMarko.IntegrationTests
{
    public class BaseFixture : WebApplicationFactory<IApiMarker>, IAsyncLifetime
    {
        public HttpClient client;

        public async Task InitializeAsync()
        {
            client = CreateClient();
            await ConfigureClientAuthentication();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");
        }

        public new Task DisposeAsync()
        {
            client.Dispose();
            return Task.CompletedTask;
        }

        private async Task ConfigureClientAuthentication()
        {
            var result = await client.PostAsJsonAsync("/login", new UserLoginDto { Email = "test_user@test.com", Password = "@PostsByMarko123" });
            var typedResult = await result.Content.ReadFromJsonAsync<RequestResult>();
            var payload = JsonConvert.DeserializeObject<LoginResponse>(typedResult!.Payload!.ToString()!);
            var token = payload!.Token;

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }
    }
}
