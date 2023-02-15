using aspnetserver.Data;
using aspnetserver.Data.Mappings;
using aspnetserver.Extensions;
using aspnetserver.Hubs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System.Text.Json.Serialization;
using static aspnetserver.Constants.AppConstants;

var builder = WebApplication.CreateBuilder(args);

var isInDocker = Environment.GetEnvironmentVariable("IS_IN_DOCKER");
var isInLocalDevelopment = builder.Environment.IsDevelopment();

var mapperConfiguration = new MapperConfiguration(mappperOptions => mappperOptions.AddProfile<UserMappingProfile>());
var allowedOrigins = builder.Configuration.GetSection("JwtConfig").GetSection("validAudiences").Get<List<string>>();
var jwtConfig = builder.Configuration.GetSection("JwtConfig");

var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbUserId = Environment.GetEnvironmentVariable("DB_USER_ID");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbPassword = Environment.GetEnvironmentVariable(variable: "MSSQL_SA_PASSWORD");

var connectionString = $"Data Source={dbHost};Initial Catalog={dbName};User ID={dbUserId};Password={dbPassword}";
if (isInDocker == null) connectionString = builder.Configuration.GetConnectionString("MySqlConnection");

#region ServicesConfiguration
builder.Host.UseSerilog();
ConfigureLogging();

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);

builder.WithCors(corsPolicyName, allowedOrigins);

builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

builder.Services.AddSignalR();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

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

        Log.Logger.Information(connectionString);
        appDbContext.Database.EnsureDeleted();
        appDbContext.Database.EnsureCreated();
    }
}

app.UseCors(corsPolicyName);
if (!isInLocalDevelopment) app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<PostHub>("/postHub");
app.MapControllers();

app.UseRateLimiting();

#endregion

app.Run();


ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration)
{
    return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
    {
        AutoRegisterTemplate = true,
        IndexFormat = $"posts-server-{DateTime.UtcNow:yyyy-MM}"
    };
}

void ConfigureLogging()
{
    var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true).Build();

    Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .Enrich.WithExceptionDetails()
        .WriteTo.Debug()
        .WriteTo.Console()
        .WriteTo.Elasticsearch(ConfigureElasticSink(configuration))
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}