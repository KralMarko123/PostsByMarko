using Microsoft.EntityFrameworkCore;
using PostsByMarko.Host.Data.Entities;

namespace PostsByMarko.Host.Data.Repositories.EmailOutbox;

public class EmailOutboxRepository(AppDbContext appDbContext) : IEmailOutboxRepository
{
    public async Task<EmailOutboxMessage?> LeaseNextAsync(
        string lockId,
        DateTime utcNow,
        TimeSpan leaseDuration,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await appDbContext.Database.BeginTransactionAsync(cancellationToken);

        var messages = await appDbContext.EmailOutboxMessages
            .FromSqlInterpolated($"""
                SELECT * FROM EmailOutboxMessages
                WHERE SentAt IS NULL
                  AND AvailableAt <= {utcNow}
                  AND (LockedUntil IS NULL OR LockedUntil <= {utcNow})
                ORDER BY CreatedAt
                LIMIT 1
                FOR UPDATE SKIP LOCKED
                """)
            .ToListAsync(cancellationToken);
        var message = messages.SingleOrDefault();

        if (message is null)
        {
            await transaction.CommitAsync(cancellationToken);
            return null;
        }

        message.LockId = lockId;
        message.LockedUntil = utcNow.Add(leaseDuration);
        await appDbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return message;
    }

    public async Task MarkSentAsync(
        Guid messageId,
        string lockId,
        DateTime sentAt,
        CancellationToken cancellationToken = default)
    {
        await appDbContext.EmailOutboxMessages
            .Where(message => message.Id == messageId && message.LockId == lockId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(message => message.SentAt, sentAt)
                .SetProperty(message => message.LockId, (string?)null)
                .SetProperty(message => message.LockedUntil, (DateTime?)null)
                .SetProperty(message => message.LastError, (string?)null), cancellationToken);
    }

    public async Task MarkFailedAsync(
        Guid messageId,
        string lockId,
        DateTime availableAt,
        string error,
        CancellationToken cancellationToken = default)
    {
        await appDbContext.EmailOutboxMessages
            .Where(message => message.Id == messageId && message.LockId == lockId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(message => message.AttemptCount, message => message.AttemptCount + 1)
                .SetProperty(message => message.AvailableAt, availableAt)
                .SetProperty(message => message.LockId, (string?)null)
                .SetProperty(message => message.LockedUntil, (DateTime?)null)
                .SetProperty(message => message.LastError, error), cancellationToken);
    }
}
