using PostsByMarko.Host.Application.Interfaces;
using PostsByMarko.Host.Data.Repositories.EmailOutbox;
using Microsoft.Extensions.Options;
using PostsByMarko.Host.Application.Configuration;

namespace PostsByMarko.Host.Application.Services;

public class EmailOutboxProcessor(
    IEmailOutboxRepository emailOutboxRepository,
    IEmailService emailService,
    TimeProvider timeProvider,
    IOptions<EmailConfig> emailConfig,
    ILogger<EmailOutboxProcessor> logger)
{
    private static readonly TimeSpan LeaseDuration = TimeSpan.FromMinutes(2);
    private static readonly TimeSpan DeliveryTimeout = TimeSpan.FromMinutes(1);
    private const int MaxErrorLength = 2000;

    public async Task<bool> ProcessNextAsync(CancellationToken cancellationToken = default)
    {
        // The sender uses the same options. A disabled sender must leave the queue untouched.
        if (!emailConfig.Value.Enabled)
        {
            return false;
        }

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
            // End the SMTP attempt before another worker can acquire the expired lease.
            var remainingTime = now.Add(DeliveryTimeout) - timeProvider.GetUtcNow().UtcDateTime;
            if (remainingTime <= TimeSpan.Zero)
            {
                throw new TimeoutException("The email delivery deadline elapsed while acquiring the lease.");
            }

            using var timeout = new CancellationTokenSource(remainingTime, timeProvider);
            using var delivery = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeout.Token);
            await emailService.SendEmailConfirmationLinkAsync(message.RecipientEmail, delivery.Token);
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
