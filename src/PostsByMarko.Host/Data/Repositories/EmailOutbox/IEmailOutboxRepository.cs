using PostsByMarko.Host.Data.Entities;

namespace PostsByMarko.Host.Data.Repositories.EmailOutbox;

public interface IEmailOutboxRepository
{
    Task<EmailOutboxMessage?> LeaseNextAsync(
        string lockId,
        DateTime utcNow,
        TimeSpan leaseDuration,
        CancellationToken cancellationToken = default);

    Task MarkSentAsync(
        Guid messageId,
        string lockId,
        DateTime sentAt,
        CancellationToken cancellationToken = default);

    Task MarkFailedAsync(
        Guid messageId,
        string lockId,
        DateTime availableAt,
        string error,
        CancellationToken cancellationToken = default);
}
