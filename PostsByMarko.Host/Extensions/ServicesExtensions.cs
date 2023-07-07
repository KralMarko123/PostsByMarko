using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PostsByMarko.Host.Data;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Host.Data.Repos.Posts;
using PostsByMarko.Host.Data.Repos.Users;
using PostsByMarko.Host.Helper;
using PostsByMarko.Host.Services;
using System.Text;

namespace PostsByMarko.Host.Extensions
{
    public static class ServicesExtensions
    {
        public static void WithCors(this WebApplicationBuilder builder, string policyName, List<string> allowedOrigins)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: policyName,
                    policy =>
                    {
                        policy
                         .WithOrigins(allowedOrigins.ToArray())
                         .AllowAnyHeader()
                         .AllowAnyMethod()
                         .AllowCredentials();
                    });
            });
        }

        public static void WithIdentity(this WebApplicationBuilder builder)
        {
            builder.Services.AddIdentity<User, IdentityRole>(user =>
            {
                user.Password.RequiredLength = 6;
                user.Password.RequireDigit = true;
                user.Password.RequireLowercase = true;
                user.Password.RequireUppercase = true;
                user.Password.RequireNonAlphanumeric = false;
                user.User.RequireUniqueEmail = true;
                user.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
        }

        public static void WithSwagger(this WebApplicationBuilder builder)
        {
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
                Array.Empty<string>()
                    }
                });
            });
        }

        public static void WithAuthentication(this WebApplicationBuilder builder, IConfigurationSection jwtConfig)
        {
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["secret"])),
                };
            });
        }

        public static void WithAuthorization(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
            });
        }

        public static void WithServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IPostsService, PostsService>();
            builder.Services.AddScoped<IUsersService, UsersService>();
            builder.Services.AddScoped<IPostsRepository, PostsRepository>();
            builder.Services.AddScoped<IUsersRepository, UsersRepository>();
            builder.Services.AddScoped<IJwtHelper, JwtHelper>();
            builder.Services.AddScoped<IEmailHelper, EmailHelper>();
        }
    }
}
