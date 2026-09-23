using PostsByMarko.Host.Application.Interfaces;
using PostsByMarko.Host.Data.Repositories.EmailOutbox;

namespace PostsByMarko.Host.Application.Services;

public class EmailOutboxProcessor(
    IEmailOutboxRepository emailOutboxRepository,
    IEmailService emailService,
    TimeProvider timeProvider,
    ILogger<EmailOutboxProcessor> logger)
{
    private static readonly TimeSpan LeaseDuration = TimeSpan.FromMinutes(2);
    private const int MaxErrorLength = 2000;

    public async Task<bool> ProcessNextAsync(CancellationToken cancellationToken = default)
    {
        var lockId = Guid.NewGuid().ToString("N");
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var message = await emailOutboxRepository.LeaseNextAsync(
            lockId, now, LeaseDuration, cancellationToken);

        if (message is null)
        {
            return false;
        }

        try
        {
            await emailService.SendEmailConfirmationLinkAsync(message.RecipientEmail, cancellationToken);
            await emailOutboxRepository.MarkSentAsync(
                message.Id, lockId, timeProvider.GetUtcNow().UtcDateTime, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            var delay = CalculateRetryDelay(message.AttemptCount + 1);
            var error = $"{exception.GetType().Name}: {exception.Message}";
            if (error.Length > MaxErrorLength)
            {
                error = error[..MaxErrorLength];
            }

            await emailOutboxRepository.MarkFailedAsync(
                message.Id,
                lockId,
                timeProvider.GetUtcNow().UtcDateTime.Add(delay),
                error,
                cancellationToken);

            logger.LogWarning(exception,
                "Confirmation email delivery failed for outbox message {MessageId}; it will be retried in {RetryDelay}.",
                message.Id,
                delay);
        }

        return true;
    }

    private static TimeSpan CalculateRetryDelay(int attemptCount)
    {
        var minutes = Math.Min(Math.Pow(2, Math.Min(attemptCount - 1, 10)), 60);
        return TimeSpan.FromMinutes(minutes);
    }
}
