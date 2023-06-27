using FluentAssertions;
using Newtonsoft.Json;
using PostsByMarko.Host.Data.Models;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace PostsByMarko.IntegrationTests
{
    [Collection("Integration Collection")]
    public class UsersControllerTests
    {
        private readonly HttpClient client;
        public UsersControllerTests(BaseFixture baseFixture)
        {
            client = baseFixture.client;
        }

        [Fact]
        public async Task should_return_list_of_all_usernames()
        {
            var result = await client.GetFromJsonAsync<RequestResult>("/getAllUsers");
            var usernames = JsonConvert.DeserializeObject<List<string>>(result!.Payload!.ToString()!);

            result.StatusCode.Should().Be(HttpStatusCode.OK);
            usernames.Should().NotBeNullOrEmpty();
        }
    }
}