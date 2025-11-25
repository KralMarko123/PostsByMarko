using PostsByMarko.Host.Data;

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
            using (var scope = app.Services.CreateScope())
            {
                var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                appDbContext.Database.EnsureDeleted();
                appDbContext.Database.EnsureCreated();

                await appDbContext.Seed();
            }
        }
    }
}
