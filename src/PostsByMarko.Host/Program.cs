
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostsByMarko.Host.Constants;
using PostsByMarko.Host.Data;
using PostsByMarko.Host.Extensions;
using PostsByMarko.Host.Hubs;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var isInDocker = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var isInLocalDevelopment = builder.Environment.IsDevelopment();
var allowedOrigins = builder.Configuration.GetSection("JwtConfig").GetSection("validAudiences").Get<List<string>>();
var jwtConfig = builder.Configuration.GetSection("JwtConfig");
var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");

#region ServicesConfiguration

builder.Host.UseSerilog();
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.WithCors(MiscConstants.CORS_POLICY_NAME, allowedOrigins!);
builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
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
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString!));
builder.WithServices();
builder.WithIdentity();
builder.Services.AddEndpointsApiExplorer();
builder.WithSwagger();
builder.WithAuthentication(jwtConfig);
builder.WithAuthorization();

#endregion

var app = builder.Build();

#region ApplicationConfiguration

if (isInLocalDevelopment)
{
    app.UseSwagger();
    app.UseSwaggerUI(swaggerUIOptions =>
    {
        swaggerUIOptions.DocumentTitle = "ASP.NET Posts Project";
        swaggerUIOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API serving a posts model.");
        swaggerUIOptions.RoutePrefix = string.Empty;
    });

    using (var scope = app.Services.CreateScope())
    {
        var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        appDbContext.Database.EnsureDeleted();
        appDbContext.Database.EnsureCreated();
    }
}

app.UseCors(MiscConstants.CORS_POLICY_NAME);

if (!isInLocalDevelopment || isInDocker != null) app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapHub<PostHub>("/postHub");
app.MapControllers();
app.UseRateLimiting();

#endregion

app.Run();

public partial class Program { }
public interface IApiMarker { }