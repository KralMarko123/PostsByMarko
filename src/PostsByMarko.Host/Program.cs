using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostsByMarko.Host.Application.Configuration;
using PostsByMarko.Host.Application.Constants;
using PostsByMarko.Host.Application.Mapping.Profiles;
using PostsByMarko.Host.Data;
using PostsByMarko.Host.Extensions;
using Serilog;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment;
var isInLocalDevelopment = environment.IsDevelopment();
var isInTest = environment.IsEnvironment("Test");

builder.Configuration.AddEnvironmentVariables();

// Define configurations here
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));
builder.Services.Configure<EmailConfig>(builder.Configuration.GetSection("EmailConfig"));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var serverVersion = new MariaDbServerVersion(new Version(10, 11, 13));
var jwtConfig = builder.Configuration.GetSection("JwtConfig").Get<JwtConfig>();

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
    options.UseMySql(connectionString!, serverVersion)
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
builder.Services.AddProblemDetails();
builder.WithSwagger();
builder.WithAuthentication(jwtConfig);
builder.WithAuthorization();

#endregion

var app = builder.Build();

#region ApplicationConfiguration

app.WithSwaggerEnabled();

if (isInLocalDevelopment || isInTest)
{
    await app.WithDatabaseReset();
}
else
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
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
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.WithMiddlewares();
app.WithHubs();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

#endregion

if (isInLocalDevelopment)
{
    Console.WriteLine("App is running locally!");
}

await app.RunAsync();

public partial class Program { }