using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PostsByMarko.Host.Application.Enums;
using PostsByMarko.Host.Application.Exceptions;
using PostsByMarko.Host.Application.Interfaces;
using PostsByMarko.Host.Application.Requests;
using PostsByMarko.Host.Application.Services;
using PostsByMarko.Host.Data.Repositories.Users;
using PostsByMarko.Host.Middlewares;

namespace PostsByMarko.UnitTests;

public class RequestValidationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void empty_post_and_message_content_is_invalid(string? content)
    {
        var message = new SendMessageRequest { Content = content! };
        Assert.False(Validator.TryValidateObject(message, new ValidationContext(message), [], true));
        var post = new UpdatePostRequest { Title = "Title", Content = content! };
        Assert.False(Validator.TryValidateObject(post, new ValidationContext(post), [], true));
    }

    [Theory]
    [InlineData(null)]
    [InlineData(ActionType.Update)]
    [InlineData((ActionType)999)]
    public async Task invalid_role_action_never_mutates_roles(ActionType? action)
    {
        var repository = new Mock<IUserRepository>(MockBehavior.Strict);
        var service = new AdminService(repository.Object, Mock.Of<ICurrentRequestAccessor>(), null!);
        await Assert.ThrowsAsync<BadRequestException>(() => service.UpdateUserRolesAsync(new UpdateUserRolesRequest
        {
            UserId = Guid.NewGuid(), Role = "Admin", ActionType = action
        }));
        repository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task unexpected_errors_do_not_expose_internal_details()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var middleware = new ExceptionHandlingMiddleware(NullLogger<ExceptionHandlingMiddleware>.Instance);
        await middleware.InvokeAsync(context, _ => throw new InvalidOperationException("private connection detail"));
        Assert.Equal(500, context.Response.StatusCode);
        context.Response.Body.Position = 0;
        var response = await new StreamReader(context.Response.Body).ReadToEndAsync();
        Assert.DoesNotContain("private connection detail", response);
        Assert.Contains("traceId", response);
        Assert.Equal("application/problem+json", context.Response.ContentType);
    }

    [Fact]
    public async Task expected_api_errors_use_problem_details_and_keep_client_message_compatibility()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/example";
        context.Response.Body = new MemoryStream();
        var middleware = new ExceptionHandlingMiddleware(NullLogger<ExceptionHandlingMiddleware>.Instance);

        await middleware.InvokeAsync(context, _ => throw new BadRequestException("Public validation message."));

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        context.Response.Body.Position = 0;
        using var response = await JsonDocument.ParseAsync(context.Response.Body);
        Assert.Equal("about:blank", response.RootElement.GetProperty("type").GetString());
        Assert.Equal("Invalid request", response.RootElement.GetProperty("title").GetString());
        Assert.Equal("Public validation message.", response.RootElement.GetProperty("detail").GetString());
        Assert.Equal("Public validation message.", response.RootElement.GetProperty("message").GetString());
        Assert.Equal("invalid_request", response.RootElement.GetProperty("code").GetString());
        Assert.Equal("/api/example", response.RootElement.GetProperty("instance").GetString());
        Assert.False(string.IsNullOrWhiteSpace(response.RootElement.GetProperty("traceId").GetString()));
    }

    [Fact]
    public async Task arbitrary_argument_errors_do_not_expose_internal_details()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var middleware = new ExceptionHandlingMiddleware(NullLogger<ExceptionHandlingMiddleware>.Instance);

        await middleware.InvokeAsync(context, _ => throw new ArgumentException("private library detail"));

        context.Response.Body.Position = 0;
        var response = await new StreamReader(context.Response.Body).ReadToEndAsync();
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        Assert.DoesNotContain("private library detail", response);
        Assert.Contains("The request is invalid.", response);
    }
}
