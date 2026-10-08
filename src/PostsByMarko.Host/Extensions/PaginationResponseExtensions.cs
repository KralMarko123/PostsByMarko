using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using PostsByMarko.Host.Application.Responses;

namespace PostsByMarko.Host.Extensions;

public static class PaginationResponseExtensions
{
    public static readonly string[] HeaderNames = ["X-Page", "X-Page-Size", "X-Total-Count", "X-Has-Next-Page"];

    public static OkObjectResult PagedOk<T>(this ControllerBase controller, PagedResult<T> result)
    {
        var headers = controller.Response.Headers;
        headers["X-Page"] = result.Page.ToString(CultureInfo.InvariantCulture);
        headers["X-Page-Size"] = result.PageSize.ToString(CultureInfo.InvariantCulture);
        headers["X-Total-Count"] = result.TotalCount.ToString(CultureInfo.InvariantCulture);
        headers["X-Has-Next-Page"] = result.HasNextPage ? "true" : "false";
        return controller.Ok(result.Items);
    }
}
