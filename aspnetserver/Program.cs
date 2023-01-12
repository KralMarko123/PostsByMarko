using aspnetserver.Data;
using aspnetserver.Data.Mappings;
using aspnetserver.Extensions;
using aspnetserver.Hubs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog.Web;
using System.Text.Json.Serialization;
using static aspnetserver.Constants.AppConstants;

var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

try
{
    logger.Info("Starting Posts-Server");

    var builder = WebApplication.CreateBuilder(args);
    var dockerFlag = Environment.GetEnvironmentVariable("IS_IN_DOCKER");

    var isInDevelopment = builder.Environment.IsDevelopment();

    var mapperConfiguration = new MapperConfiguration(mappperOptions => mappperOptions.AddProfile<UserMappingProfile>());
    var allowedOrigins = builder.Configuration.GetSection("JwtConfig").GetSection("validAudiences").Get<List<string>>();
    var jwtConfig = builder.Configuration.GetSection("JwtConfig");

    var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
    var dbUserId = Environment.GetEnvironmentVariable("DB_USER_ID");
    var dbName = Environment.GetEnvironmentVariable("DB_NAME");
    var dbPassword = Environment.GetEnvironmentVariable(variable: "MSSQL_SA_PASSWORD");

    var connectionString = $"Data Source={dbHost};Initial Catalog={dbName};User ID={dbUserId};Password={dbPassword}";
    if (dockerFlag == null) connectionString = builder.Configuration.GetConnectionString("MySqlConnection");


    logger.Info("Configuring Services for Posts-Server");

    #region ServicesConfiguration
    builder.WithCors(corsPolicyName, allowedOrigins);

    builder.Host.UseNLog();

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

    if (isInDevelopment)
    {
        logger.Info("Adding Swagger suppoort and initial DB state for development");

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


            logger.Info(connectionString);

            appDbContext.Database.EnsureDeleted();
            appDbContext.Database.EnsureCreated();
        }
    }

    app.UseCors(corsPolicyName);
    if (!isInDevelopment) app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapHub<PostHub>("/postHub");
    app.MapControllers();

    app.UseRateLimiting();

    #endregion

    app.Run();

    logger.Info("Posts-Server successfully started");

}
catch (Exception ex)
{
    logger.Error(ex, $"Posts-Server has stopped due to an exception with message: {ex.Message}");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}