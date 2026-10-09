using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using PostsByMarko.Host.Application.Configuration;
using PostsByMarko.Host.Application.Helper;
using PostsByMarko.Host.Application.Services;
using PostsByMarko.Host.Data;
using PostsByMarko.Host.Data.Entities;
using PostsByMarko.Host.Data.Repositories.EmailOutbox;
using PostsByMarko.Host.Data.Repositories.Users;
using PostsByMarko.Test.Shared.Constants;
using Xunit;

namespace PostsByMarko.IntegrationTests;

[Collection("IntegrationCollection")]
public class EmailOutboxTests(PostsByMarkoApiFactory factory) : IAsyncLifetime
{
    public Task InitializeAsync() => factory.RecreateAndSeedDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task disabled_delivery_preserves_pending_message_and_enabled_delivery_marks_it_sent()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var services = scope.ServiceProvider;
        var users = services.GetRequiredService<UserManager<User>>();
        var user = (await users.FindByEmailAsync(TestingConstants.UNCONFIRMED_USER_EMAIL))!;
        var repository = services.GetRequiredService<IUserRepository>();
        await repository.QueueConfirmationEmailAsync(user);
        var db = services.GetRequiredService<AppDbContext>();
        var original = await db.EmailOutboxMessages.AsNoTracking().SingleAsync();
        var sender = new RecordingEmailHelper();
        var email = new EmailService(sender, repository, services.GetRequiredService<IOptions<ApplicationUrlConfig>>());
        var settings = new EmailConfig { Enabled = false };
        var processor = new EmailOutboxProcessor(services.GetRequiredService<IEmailOutboxRepository>(),
            email, System.TimeProvider.System, Options.Create(settings), NullLogger<EmailOutboxProcessor>.Instance);

        // Act
        Assert.False(await processor.ProcessNextAsync());
        var pending = await db.EmailOutboxMessages.AsNoTracking().SingleAsync();
        // Assert
        Assert.Equal(original.Id, pending.Id);
        Assert.Equal(original.AvailableAt, pending.AvailableAt);
        Assert.Equal(0, pending.AttemptCount);
        Assert.Null(pending.LockId);
        Assert.Null(pending.LockedUntil);
        Assert.Null(pending.SentAt);
        Assert.Null(sender.Recipient);

        settings.Enabled = true;
        Assert.True(await processor.ProcessNextAsync());
        var sent = await db.EmailOutboxMessages.AsNoTracking().SingleAsync();
        Assert.Equal(original.Id, sent.Id);
        Assert.NotNull(sent.SentAt);
        Assert.Null(sent.LockId);
        Assert.Null(sent.LockedUntil);
        Assert.Equal(user.Email, sender.Recipient);
        Assert.Contains("/api/auth/confirm?", sender.Content!.TextBody);
        Assert.Contains("Confirm your email</a>", sender.Content.HtmlBody);
        Assert.False(await processor.ProcessNextAsync());
    }

    private sealed class RecordingEmailHelper : IEmailHelper
    {
        public string? Recipient { get; private set; }
        public EmailContent? Content { get; private set; }

        public Task SendEmailAsync(string firstName, string lastName, string emailToSendTo, string subject,
            EmailContent content, CancellationToken cancellationToken = default)
        {
            Recipient = emailToSendTo;
            Content = content;
            return Task.CompletedTask;
        }
    }
}
