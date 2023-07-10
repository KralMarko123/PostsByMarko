using System.Net;

namespace PostsByMarko.Host.Data.Models
{
    public class RequestResult
    {
        public HttpStatusCode StatusCode { get; set; }
        public string? Message { get; set; } = string.Empty;
        public object? Payload { get; set; }
    }
}
