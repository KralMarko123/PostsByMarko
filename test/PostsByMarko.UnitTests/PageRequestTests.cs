using System.ComponentModel.DataAnnotations;
using PostsByMarko.Host.Application.Exceptions;
using PostsByMarko.Host.Application.Requests;

namespace PostsByMarko.UnitTests;

public class PageRequestTests
{
    [Theory]
    [InlineData(0, 50)]
    [InlineData(-1, 50)]
    [InlineData(1, 0)]
    [InlineData(1, -1)]
    [InlineData(1, 101)]
    [InlineData(int.MaxValue, 100)]
    public void invalid_paging_is_rejected_for_http_binding_and_internal_calls(int page, int pageSize)
    {
        var request = new PageRequest { Page = page, PageSize = pageSize };
        Assert.False(Validator.TryValidateObject(request, new ValidationContext(request), [], true));
        Assert.Throws<BadRequestException>(request.EnsureValid);
    }

    [Fact]
    public void large_valid_offsets_do_not_overflow()
    {
        var request = new PageRequest { Page = int.MaxValue, PageSize = 1 };
        request.EnsureValid();
        Assert.Equal(int.MaxValue - 1, request.GetOffset());
    }
}
