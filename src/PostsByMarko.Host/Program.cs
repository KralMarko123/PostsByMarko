using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostsByMarko.Host.Application.Configuration;
using PostsByMarko.Host.Application.Constants;
using PostsByMarko.Host.Application.Mapping.Profiles;
using PostsByMarko.Host.Data;
using PostsByMarko.Host.Extensions;
using PostsByMarko.Host.Middlewares;
using Serilog;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment;
var isInLocalDevelopment = environment.IsDevelopment();
var isInTest = environment.IsEnvironment("Test");

builder.Configuration.AddEnvironmentVariables();

var jwtConfig = builder.WithValidatedConfiguration();
var serverVersion = new MariaDbServerVersion(new Version(10, 11, 13));

#region ServicesConfiguration

builder.Host.UseSerilog((context, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console());
builder.WithCors(MiscConstants.CORS_POLICY_NAME, jwtConfig.ValidAudiences);
// Apply after MVC's defaults so they cannot replace the shared validation response.
builder.Services.PostConfigure<ApiBehaviorOptions>(options =>
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(entry => entry.Value!.Errors.Count > 0)
            .ToDictionary(
                entry => entry.Key,
                entry => entry.Value!.Errors.Select(error => error.ErrorMessage).ToArray());
        var result = new BadRequestObjectResult(
            ApiProblemDetailsFactory.CreateValidation(context.HttpContext, errors));
        result.ContentTypes.Add("application/problem+json");
        return result;
    });
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (rejectedContext, cancellationToken) =>
    {
        await ApiProblemDetailsFactory.WriteAsync(
            rejectedContext.HttpContext,
            StatusCodes.Status429TooManyRequests,
            "Too many requests",
            "Too many requests were sent. Please try again later.",
            "rate_limit_exceeded");
    };
    options.AddPolicy("authentication", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = builder.Configuration.GetValue("Authentication:RequestsPerMinute", 10),
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        }));
});
builder.Services.AddSignalR(options =>
{
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
    options.KeepAliveInterval = TimeSpan.FromSeconds(10);
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});
builder.Services.AddDbContext<AppDbContext>((services, options) =>
    options.UseMySql(services.GetRequiredService<IOptions<DatabaseConfig>>().Value.DefaultConnection, serverVersion)
);
builder.WithHealthChecks();
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<RegistrationProfile>();
    cfg.AddProfile<UserProfile>();
    cfg.AddProfile<PostProfile>();   
    cfg.AddProfile<MessagingProfile>();
});
builder.Services.AddHttpContextAccessor();
builder.WithAppServices(enableBackgroundServices: !isInTest);
builder.WithIdentity();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddProblemDetails();
builder.WithSwagger();
builder.WithAuthentication(jwtConfig);
builder.WithAuthorization();

#endregion

var app = builder.Build();
// Validate every configuration group before migrations, seeding, or background workers can run.
app.Services.GetRequiredService<IStartupValidator>().Validate();

#region ApplicationConfiguration

app.WithSwaggerEnabled();

if (isInTest)
{
    await app.WithDatabaseReset();
}
else
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await scope.ServiceProvider.InitializePersistentIdentityAsync(builder.Configuration, isInLocalDevelopment);
}

app.UseCors(MiscConstants.CORS_POLICY_NAME);

if (!isInLocalDevelopment && !isInTest)
{
    app.UseHttpsRedirection();
}

// Enable the developer exception page only in the Development environment
if (isInLocalDevelopment)
{
    app.UseDeveloperExceptionPage();
}
else
{
    // Configure production error handling
    app.UseHsts();
}

app.WithMiddlewares();
app.WithHubs();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();
app.MapApplicationHealthChecks();

#endregion

if (isInLocalDevelopment)
{
    Console.WriteLine("App is running locally!");
}

await app.RunAsync();

public partial class Program { }
