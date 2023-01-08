using System.Net;

namespace aspnetserver.Data.Models
{
    public class RequestResult
    {
        public HttpStatusCode StatusCode { get; set; }
        public string? Message { get; set; }
        public object? Payload { get; set; }
    }
}
