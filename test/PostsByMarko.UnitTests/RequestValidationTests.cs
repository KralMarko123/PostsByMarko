using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PostsByMarko.Host.Application.Enums;
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
        await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateUserRolesAsync(new UpdateUserRolesRequest
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
    }
}
