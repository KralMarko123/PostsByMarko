using aspnetserver.Data;
using aspnetserver.Data.Mappings;
using aspnetserver.Data.Repos.Posts;
using aspnetserver.Data.Repos.Users;
using aspnetserver.Extensions;
using aspnetserver.Helper;
using aspnetserver.Hubs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using static aspnetserver.Constants.AppConstants;

var mapperConfiguration = new MapperConfiguration(mappperOptions => mappperOptions.AddProfile<UserMappingProfile>());
var builder = WebApplication.CreateBuilder(args);
var environment = builder.Configuration["Environment"];
var connectionString = "";
var allowedOrigins = new List<string>();
IConfigurationSection jwtConfig = null;

switch (environment)
{
    case "DEV":
        allowedOrigins = allowedDevOrigins;
        jwtConfig = builder.Configuration.GetSection("DevJwtConfig");
        connectionString = builder.Configuration.GetConnectionString("DevConnection");
        break;
    case "PRD":
        allowedOrigins = allowedPrdOrigins;
        jwtConfig = builder.Configuration.GetSection("JwtConfig");
        connectionString = builder.Configuration.GetConnectionString("PrdConnection");
        break;
    default:
        break;
}

#region ServicesConfiguration

builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
builder.Services.AddSingleton(mapperConfiguration.CreateMapper());

builder.WithCors(corsPolicyName, allowedOrigins);

builder.Services.AddSignalR();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IPostsRepository, PostsRepository>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IJwtHelper, JwtHelper>();

builder.WithIdentity();

builder.Services.AddEndpointsApiExplorer();
builder.WithSwagger();
builder.WithAuthentication(jwtConfig);
builder.WithAuthorization();

#endregion

var app = builder.Build();

#region ApplicationConfiguration

app.UseSwagger();
app.UseSwaggerUI(swaggerUIOptions =>
{
    swaggerUIOptions.DocumentTitle = "ASP.NET Posts Project";
    swaggerUIOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API serving a posts model.");
    swaggerUIOptions.RoutePrefix = string.Empty;
});

app.UseCors(corsPolicyName);
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapHub<PostHub>("/postHub");
app.MapControllers();

app.UseRateLimiting();

#endregion

app.Run();

