namespace PostsByMarko.Host.Application.Services;

public class EmailOutboxWorker(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<EmailOutboxWorker> logger) : BackgroundService
{
    private static readonly TimeSpan IdleDelay = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan ErrorDelay = TimeSpan.FromSeconds(15);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = serviceScopeFactory.CreateAsyncScope();
                var processor = scope.ServiceProvider.GetRequiredService<EmailOutboxProcessor>();
                var processed = await processor.ProcessNextAsync(stoppingToken);

                if (!processed)
                {
                    await Task.Delay(IdleDelay, stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "The confirmation email outbox worker failed while polling for messages.");
                await Task.Delay(ErrorDelay, stoppingToken);
            }
        }
    }
}
