namespace PostsByMarko.Host.Application.Responses;

public sealed record PagedResult<T>(List<T> Items, int TotalCount, int Page, int PageSize)
{
    public bool HasNextPage => (long)Page * PageSize < TotalCount;

    public PagedResult<TResult> Map<TResult>(Func<T, TResult> map) =>
        new(Items.Select(map).ToList(), TotalCount, Page, PageSize);
}
