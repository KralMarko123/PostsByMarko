using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostsByMarko.Host.Constants;
using PostsByMarko.Host.Data;
using PostsByMarko.Host.Extensions;
using PostsByMarko.Host.Hubs;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var isInLocalDevelopment = builder.Environment.IsDevelopment();
var isInTest = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.Equals("Test");
var allowedOrigins = builder.Configuration.GetSection("JwtConfig").GetSection("validAudiences").Get<List<string>>();
var jwtConfig = builder.Configuration.GetSection("JwtConfig");
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

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
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseNpgsql(connectionString!)
);
builder.WithServices();
builder.WithIdentity();
builder.Services.AddEndpointsApiExplorer();
builder.WithSwagger();
builder.WithAuthentication(jwtConfig);
builder.WithAuthorization();

#endregion

var app = builder.Build();

#region ApplicationConfiguration



if (isInLocalDevelopment || isInTest.GetValueOrDefault(false))
{
    app.WithSwaggerEnabled();
    app.WithDatabaseReset();
}

app.UseCors(MiscConstants.CORS_POLICY_NAME);

if (!isInLocalDevelopment && !isInTest.GetValueOrDefault(false))
{
    app.UseHttpsRedirection();
    app.UseRateLimiting();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapHub<PostHub>("/postHub");
app.MapHub<MessageHub>("/messageHub");
app.MapControllers();

#endregion

if (isInLocalDevelopment) Console.WriteLine("App is running locally!");

app.Run();

public partial class Program { }
public interface IApiMarker { }