using aspnetserver.Data;
using aspnetserver.Data.Mappings;
using aspnetserver.Data.Models;
using aspnetserver.Data.Repos.Posts;
using aspnetserver.Data.Repos.Users;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var mapperConfiguration = new MapperConfiguration(mappperOptions => mappperOptions.AddProfile<UserMappingProfile>());
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var jwtConfig = builder.Configuration.GetSection("JwtConfig");
var postsCorsPolicy = "postsCorsPolicy";


#region ServicesConfiguration

builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
builder.Services.AddSingleton(mapperConfiguration.CreateMapper());
builder.Services.AddCors(options =>
{
    options.AddPolicy(postsCorsPolicy,
        policy =>
        {
            policy
             .WithOrigins("http://localhost:3000", "https://delightful-stone-08266f803.2.azurestaticapps.net", "https://posts.markomarkovikj.com")
             .AllowAnyHeader()
             .AllowAnyMethod();
        });
});
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IPostsRepository, PostsRepository>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();

builder.Services.AddIdentity<User, IdentityRole>(user =>
{
    user.Password.RequireDigit = true;
    user.Password.RequireLowercase = true;
    user.Password.RequireUppercase = true;
    user.Password.RequireNonAlphanumeric = false;
    user.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swaggerOptions =>
{
    swaggerOptions.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ASP.NET Posts Project",
        Version = "v1",
        Description = "Posts API Services",
        Contact = new OpenApiContact { Name = "Marko Markovikj" }
    });
    swaggerOptions.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    swaggerOptions.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    swaggerOptions.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuers = jwtConfig.GetSection("validIssuers").Get<List<string>>(),
        ValidAudiences = jwtConfig.GetSection("validAudiences").Get<List<string>>(),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["secret"]))
    };
});
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
    .RequireAuthenticatedUser()
    .Build();
});



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

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.UseCors(postsCorsPolicy);
app.MapControllers();

#endregion

app.Run();

