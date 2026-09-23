using PostsByMarko.Host.Application.Exceptions;
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
                var error = MapException(ex);
                if (error.Status >= StatusCodes.Status500InternalServerError)
                {
                    logger.LogError(ex, "Unhandled exception occurred for request {Method} {Path}",
                        context.Request.Method, context.Request.Path);
                }
                else
                {
                    logger.LogInformation("Request {Method} {Path} failed with status {StatusCode} and error {ErrorCode}",
                        context.Request.Method, context.Request.Path, error.Status, error.Code);
                }

                await ApiProblemDetailsFactory.WriteAsync(
                    context, error.Status, error.Title, error.Detail, error.Code);
            }
        }

        private static ApiError MapException(Exception exception)
        {
            return exception switch
            {
                ResourceNotFoundException => new(404, "Resource not found", exception.Message, "resource_not_found"),
                KeyNotFoundException => new(404, "Resource not found", "The requested resource was not found.", "resource_not_found"),
                BadRequestException => new(400, "Invalid request", exception.Message, "invalid_request"),
                ValidationException => new(400, "Invalid request", "One or more values are invalid.", "validation_failed"),
                ArgumentException => new(400, "Invalid request", "The request is invalid.", "invalid_request"),
                ForbiddenException => new(403, "Forbidden", exception.Message, "forbidden"),
                UnauthorizedAccessException => new(403, "Forbidden", "You are not allowed to perform this action.", "forbidden"),
                AuthException => new(401, "Authentication failed", exception.Message, "authentication_failed"),
                ConflictException => new(409, "Conflict", exception.Message, "conflict"),
                _ => new(500, "Server error", "An unexpected error occurred.", "internal_error")
            };
        }

        private sealed record ApiError(int Status, string Title, string Detail, string Code);
    }
}
