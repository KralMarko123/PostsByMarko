using PostsByMarko.Host.Application.Hubs;
using PostsByMarko.Host.Data;
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
            using var scope = app.Services.CreateScope();
            var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await appDbContext.Database.EnsureDeletedAsync();
            await appDbContext.Database.EnsureCreatedAsync();
            await appDbContext.Seed();
        }

        public static void WithMiddlewares(this WebApplication app)
        {
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
