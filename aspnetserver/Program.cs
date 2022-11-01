using aspnetserver.Data;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddCors(options =>
{
    options.AddPolicy("CORSPolicy",
        builder =>
        {
            builder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithOrigins("http://localhost:3000", "https://appname.azurestaticapps.net");
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swaggerOptions =>
{
    swaggerOptions.SwaggerDoc("v1", new OpenApiInfo { Title = "ASP.NET React Tutorial", Version = "v1" });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(swaggerUIOptions =>
{
    swaggerUIOptions.DocumentTitle = "ASP.NET React Tutorial";
    swaggerUIOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API serving a very simple post model.");
    swaggerUIOptions.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseCors("CORSPolicy");

//APIs

app.MapGet("/get-all-posts", async () => await PostsRepository.GetPostsAsync())
    .WithTags(tags: "Posts Endpoint");

app.MapGet("/get-post-by-id/{postId}", async (int postId) =>
{
    Post postToReturn = await PostsRepository.GetPostByIdAsync(postId);

    if (postToReturn != null) return Results.Ok(postToReturn);
    else return Results.NotFound($"Post with id: {postId} was not found.");

}).WithTags(tags: "Posts Endpoint");

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

//END OF APIs

app.Run();

