using System.ComponentModel.DataAnnotations;

namespace PostsByMarko.Host.Data.Entities;

public class EmailOutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }

    [MaxLength(256)]
    public string RecipientEmail { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime AvailableAt { get; set; } = DateTime.UtcNow;
    public DateTime? SentAt { get; set; }
    public int AttemptCount { get; set; }

    [MaxLength(64)]
    public string? LockId { get; set; }

    public DateTime? LockedUntil { get; set; }

    [MaxLength(2000)]
    public string? LastError { get; set; }
}
