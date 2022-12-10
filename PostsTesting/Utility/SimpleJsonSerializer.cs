using RestSharp;
using System.Text.Json;

namespace PostsTesting.Utility
{
    public static class SimpleJsonSerializer
    {
        public static string Serialize(object obj) => JsonSerializer.Serialize(obj);
        public static T Deserialize<T>(RestResponse response) => JsonSerializer.Deserialize<T>(response.Content);
    };
}
