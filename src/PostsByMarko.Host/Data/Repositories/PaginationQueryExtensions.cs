using Microsoft.EntityFrameworkCore;
using PostsByMarko.Host.Application.Requests;
using PostsByMarko.Host.Application.Responses;

namespace PostsByMarko.Host.Data.Repositories;

public static class PaginationQueryExtensions
{
    public static async Task<PagedResult<T>> ToPageAsync<T>(this IOrderedQueryable<T> query,
        PageRequest page, CancellationToken cancellationToken = default)
    {
        page.EnsureValid();
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip(page.GetOffset()).Take(page.PageSize).ToListAsync(cancellationToken);
        return new(items, total, page.Page, page.PageSize);
    }
}
