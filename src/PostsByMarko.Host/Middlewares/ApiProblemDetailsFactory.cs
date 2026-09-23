using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace PostsByMarko.Host.Middlewares;

public static class ApiProblemDetailsFactory
{
    private static readonly JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web);

    public static ProblemDetails Create(
        HttpContext context,
        int status,
        string title,
        string detail,
        string code)
    {
        var problemDetails = new ProblemDetails
        {
            Type = "about:blank",
            Title = title,
            Status = status,
            Detail = detail,
            Instance = context.Request.Path
        };

        AddExtensions(problemDetails, context, code, detail);
        return problemDetails;
    }

    public static HttpValidationProblemDetails CreateValidation(
        HttpContext context,
        IDictionary<string, string[]> errors)
    {
        const string detail = "One or more fields are invalid.";
        var problemDetails = new HttpValidationProblemDetails(errors)
        {
            Type = "about:blank",
            Title = "Invalid request",
            Status = StatusCodes.Status400BadRequest,
            Detail = detail,
            Instance = context.Request.Path
        };

        AddExtensions(problemDetails, context, "validation_failed", detail);
        return problemDetails;
    }

    public static Task WriteAsync(
        HttpContext context,
        int status,
        string title,
        string detail,
        string code)
    {
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";
        return JsonSerializer.SerializeAsync(
            context.Response.Body,
            Create(context, status, title, detail, code),
            serializerOptions,
            context.RequestAborted);
    }

    private static void AddExtensions(
        ProblemDetails problemDetails,
        HttpContext context,
        string code,
        string message)
    {
        problemDetails.Extensions["code"] = code;
        problemDetails.Extensions["traceId"] = context.TraceIdentifier;
        problemDetails.Extensions["message"] = message;
    }
}
