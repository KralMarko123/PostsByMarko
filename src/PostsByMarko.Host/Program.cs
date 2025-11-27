using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PostsByMarko.Host.Application.Configuration;
using PostsByMarko.Host.Application.Constants;
using PostsByMarko.Host.Application.Hubs;
using PostsByMarko.Host.Application.Mapping.Profiles;
using PostsByMarko.Host.Data;
using PostsByMarko.Host.Extensions;
using Serilog;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment;
var isInLocalDevelopment = environment.IsDevelopment();
var isInTest = environment.IsEnvironment("Test");

if (isInLocalDevelopment || isInTest)
{
    var projectRoot = Directory.GetCurrentDirectory();
    var envPath = isInTest ? 
        Path.GetFullPath(Path.Combine(projectRoot, "../../../../../", ".env"))
        : Path.GetFullPath(Path.Combine(projectRoot, "../../", ".env"));

    Console.WriteLine($"Loading .env from: {envPath}");

    if (File.Exists(envPath))
    {
        Env.Load(envPath);
    }
    else
    {
        throw new ArgumentException("Missing '.env' file for configuration!");
    }
}

builder.Configuration.AddEnvironmentVariables();

// Define configurations here
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var jwtConfig = builder.Configuration.GetSection("JwtConfig").Get<JwtConfig>();
builder.Services.Configure<EmailConfig>(builder.Configuration.GetSection("EmailConfig"));

#region ServicesConfiguration

builder.Host.UseSerilog();
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.WithCors(MiscConstants.CORS_POLICY_NAME, jwtConfig!.ValidAudiences!);
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
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<RegistrationProfile>();
    cfg.AddProfile<UserProfile>();
    cfg.AddProfile<PostProfile>();   
    cfg.AddProfile<MessagingProfile>();
});
builder.Services.AddHttpContextAccessor();
builder.WithAppServices();
builder.WithIdentity();
builder.Services.AddEndpointsApiExplorer();
builder.WithSwagger();
builder.WithAuthentication(jwtConfig);
builder.WithAuthorization();

#endregion

var app = builder.Build();

#region ApplicationConfiguration



if (isInLocalDevelopment || isInTest)
{
    app.WithSwaggerEnabled();
    
    await app.WithDatabaseReset();
}

app.UseCors(MiscConstants.CORS_POLICY_NAME);

if (!isInLocalDevelopment && !isInTest)
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapHub<PostHub>("/postHub");
app.MapHub<MessageHub>("/messageHub");
app.MapControllers();

#endregion

if (isInLocalDevelopment) Console.WriteLine("App is running locally!");

await app.RunAsync();

public partial class Program { }