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
        // Arrange
        var request = new PageRequest { Page = page, PageSize = pageSize };
        // Act
        var isValid = Validator.TryValidateObject(request, new ValidationContext(request), [], true);
        var exception = Record.Exception(request.EnsureValid);
        // Assert
        Assert.False(isValid);
        Assert.IsType<BadRequestException>(exception);
    }

    [Fact]
    public void large_valid_offsets_do_not_overflow()
    {
        // Arrange
        var request = new PageRequest { Page = int.MaxValue, PageSize = 1 };
        // Act
        request.EnsureValid();
        // Assert
        Assert.Equal(int.MaxValue - 1, request.GetOffset());
    }
}
