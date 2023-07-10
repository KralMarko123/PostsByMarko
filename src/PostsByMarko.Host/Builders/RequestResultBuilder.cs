using PostsByMarko.Host.Data.Models;
using System.Net;
namespace PostsByMarko.Host.Builders
{
    public class RequestResultBuilder
    {
        private readonly RequestResult requestResult;

        public RequestResultBuilder()
        {
            requestResult = new RequestResult();
        }

        public RequestResultBuilder Ok()
        {
            requestResult.StatusCode = HttpStatusCode.OK;
            return this;
        }
        public RequestResultBuilder NotFound()
        {
            requestResult.StatusCode = HttpStatusCode.NotFound;
            return this;
        }

        public RequestResultBuilder Unauthorized()
        {
            requestResult.StatusCode = HttpStatusCode.Unauthorized;
            return this;
        }
        public RequestResultBuilder Forbidden()
        {
            requestResult.StatusCode = HttpStatusCode.Forbidden;
            return this;
        }

        public RequestResultBuilder BadRequest()
        {
            requestResult.StatusCode = HttpStatusCode.BadRequest;
            return this;
        }

        public RequestResultBuilder Created()
        {
            requestResult.StatusCode = HttpStatusCode.Created;
            return this;
        }

        public RequestResultBuilder WithMessage(string message)
        {
            requestResult.Message = message;
            return this;
        }

        public RequestResultBuilder WithPayload(object payload)
        {
            requestResult.Payload = payload;
            return this;
        }

        public RequestResult Build()
        {
            return requestResult;
        }
    }
}
