using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PostsByMarko.Host.Application.Health;

namespace PostsByMarko.Host.Extensions;

public static class HealthCheckExtensions
{
    public static void WithHealthChecks(this WebApplicationBuilder builder) =>
        builder.Services.AddHealthChecks().AddCheck<DatabaseReadinessHealthCheck>(
            "database", tags: ["ready"], timeout: TimeSpan.FromSeconds(5));

    public static void MapApplicationHealthChecks(this WebApplication app)
    {
        // An empty liveness check only confirms the process can serve a request.
        app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false }).AllowAnonymous();
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = registration => registration.Tags.Contains("ready"),
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status503ServiceUnavailable,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            }
        }).AllowAnonymous();
        // The default plain-text writer exposes only Healthy/Unhealthy, never exception or connection details.
    }
}
