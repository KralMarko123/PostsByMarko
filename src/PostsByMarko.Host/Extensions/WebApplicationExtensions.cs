using PostsByMarko.Host.Application.Hubs;
using PostsByMarko.Host.Data;
using Microsoft.EntityFrameworkCore;
using PostsByMarko.Host.Middlewares;

namespace PostsByMarko.Host.Extensions
{
    public static class WebApplicationExtensions
    {
        public static void WithSwaggerEnabled(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(swaggerUIOptions =>
            {
                swaggerUIOptions.DocumentTitle = "ASP.NET Posts Project";
                swaggerUIOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API serving a posts model.");
                swaggerUIOptions.RoutePrefix = string.Empty;
            });
        }

        public static async Task WithDatabaseReset(this WebApplication app)
        {
            if (!app.Environment.IsEnvironment("Test"))
            {
                throw new InvalidOperationException("Database reset is restricted to the Test environment.");
            }

            using var scope = app.Services.CreateScope();
            var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var databaseName = appDbContext.Database.GetDbConnection().Database;

            if (!databaseName.Contains("test", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Refusing to reset database '{databaseName}' because its name is not test-scoped.");
            }

            await appDbContext.Database.EnsureDeletedAsync();
            await appDbContext.Database.EnsureCreatedAsync();
            await appDbContext.Seed();
        }

        public static void WithMiddlewares(this WebApplication app)
        {
            app.UseStatusCodePages(async statusCodeContext =>
            {
                var response = statusCodeContext.HttpContext.Response;
                var error = response.StatusCode switch
                {
                    StatusCodes.Status404NotFound => ("Resource not found", "The requested endpoint was not found.", "endpoint_not_found"),
                    StatusCodes.Status405MethodNotAllowed => ("Method not allowed", "This HTTP method is not supported for the requested endpoint.", "method_not_allowed"),
                    _ => ("Request failed", "The request could not be completed.", "request_failed")
                };

                await ApiProblemDetailsFactory.WriteAsync(
                    statusCodeContext.HttpContext,
                    response.StatusCode,
                    error.Item1,
                    error.Item2,
                    error.Item3);
            });
            app.UseMiddleware<ExceptionHandlingMiddleware>();
        }

        public static void WithHubs(this WebApplication app)
        {
            app.MapHub<AdminHub>("/adminHub");
            app.MapHub<PostHub>("/postHub");
            app.MapHub<MessageHub>("/messageHub");
        }
    }
}
