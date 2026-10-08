using System.ComponentModel.DataAnnotations;
using PostsByMarko.Host.Application.Exceptions;

namespace PostsByMarko.Host.Application.Requests;

public sealed class PageRequest : IValidatableObject
{
    public const int DefaultPageSize = 50;
    public const int MaximumPageSize = 100;

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, MaximumPageSize)]
    public int PageSize { get; set; } = DefaultPageSize;

    public int GetOffset() => checked((Page - 1) * PageSize);

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (((long)Page - 1) * PageSize > int.MaxValue)
            yield return new ValidationResult("The requested page is too large.", [nameof(Page)]);
    }

    public void EnsureValid()
    {
        if (!Validator.TryValidateObject(this, new ValidationContext(this), [], validateAllProperties: true))
            throw new BadRequestException("Page must be positive, pageSize must be between 1 and 100, and the page offset must fit within an integer.");
    }
}
