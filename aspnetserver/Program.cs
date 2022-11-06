using aspnetserver.Data;
using aspnetserver.Data.Mappings;
using aspnetserver.Data.Models;
using aspnetserver.Data.Models.Dtos;
using aspnetserver.Data.Repos.Posts;
using aspnetserver.Data.Repos.Users;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

var mapperConfiguration = new MapperConfiguration(mappperOptions => mappperOptions.AddProfile<UserMappingProfile>());
var builder = WebApplication.CreateBuilder(args);

#region ServicesConfiguration

builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
builder.Services.AddSingleton(mapperConfiguration.CreateMapper());

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


builder.Services.AddCors(options =>
{
    options.AddPolicy("CORSPolicy",
        builder =>
        {
            builder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithOrigins("http://localhost:3000", "https://delightful-stone-08266f803.2.azurestaticapps.net", "https://posts.markomarkovikj.com");
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swaggerOptions =>
{
    swaggerOptions.SwaggerDoc("v1", new OpenApiInfo { Title = "ASP.NET React Tutorial", Version = "v1" });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
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
    swaggerUIOptions.DocumentTitle = "ASP.NET React Tutorial";
    swaggerUIOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API serving a very simple post model.");
    swaggerUIOptions.RoutePrefix = string.Empty;
});

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.UseCors("CORSPolicy");

#endregion

#region APIs

#region PostAPIs

app.MapGet("/get-all-posts", async () => await PostsRepository.GetPostsAsync())
    .WithTags(tags: "Posts Endpoint").AllowAnonymous();

app.MapGet("/get-post-by-id/{postId}", async (int postId) =>
{
    Post postToReturn = await PostsRepository.GetPostByIdAsync(postId);

    if (postToReturn != null) return Results.Ok(postToReturn);
    else return Results.NotFound($"Post with id: {postId} was not found.");

}).WithTags(tags: "Posts Endpoint").AllowAnonymous();

app.MapPost("/create-post", async (Post postToCreate) =>
{
    bool postCreatedSuccessfully = false;
    if (postToCreate.Title.Length > 0 && postToCreate.Content.Length > 0) postCreatedSuccessfully = await PostsRepository.CreatePostAsync(postToCreate);

    if (postCreatedSuccessfully) return Results.Ok("Post was created successfully.");
    else return Results.BadRequest("Error during post creation.");

}).WithTags(tags: "Posts Endpoint");

app.MapPut("/update-post", async (Post postToUpdate) =>
{
    bool postUpdatedSuccessfully = await PostsRepository.UpdatePostAsync(postToUpdate);

    if (postUpdatedSuccessfully) return Results.Ok("Post was updated successfully.");
    else return Results.BadRequest("Error during post update.");

}).WithTags(tags: "Posts Endpoint");

app.MapDelete("/delete-post-by-id/{postId}", async (int postId) =>
{
    bool postDeletedSuccessfully = await PostsRepository.DeletePostAsync(postId);

    if (postDeletedSuccessfully) return Results.Ok("Post was deleted successfully.");
    else return Results.BadRequest("Error during post deletion.");

}).WithTags(tags: "Posts Endpoint");

#endregion

#region AuthAPIs

app.MapPost("/register", async (UserRegistrationDto userToRegister) =>
{
    var userCreated = await UsersRepository.RegisterUserAsync(userToRegister);

    if (userCreated.Succeeded) return Results.CreatedAtRoute("User registered successfully");
    else return Results.BadRequest(new BadRequestObjectResult(userCreated));

}).WithTags("Auth Endpoint").AllowAnonymous();

#endregion

#endregion

app.Run();

