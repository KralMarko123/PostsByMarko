using PostsByMarko.Host.Application.Exceptions;
using System.Net;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;

namespace PostsByMarko.Host.Middlewares
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        {
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                // The caller disconnected; there is no response to send.
            }
            catch (Exception ex) when (!context.Response.HasStarted)
            {
                logger.LogError(ex, "Unhandled exception occurred");

                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var statusCode = ex switch
            {
                KeyNotFoundException => HttpStatusCode.NotFound,
                ValidationException => HttpStatusCode.BadRequest,
                ArgumentException => HttpStatusCode.BadRequest,
                UnauthorizedAccessException => HttpStatusCode.Forbidden,
                AuthException => HttpStatusCode.Unauthorized,
                _ => HttpStatusCode.InternalServerError
            };

            var problemDetails = new
            {
                message = statusCode == HttpStatusCode.InternalServerError ? "An unexpected error occurred." : ex.Message,
                status = (int)statusCode,
                traceId = context.TraceIdentifier
            };

            var responseBody = JsonSerializer.Serialize(problemDetails);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(responseBody);
        }
    }
}
