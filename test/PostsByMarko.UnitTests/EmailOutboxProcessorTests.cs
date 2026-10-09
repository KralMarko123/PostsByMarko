using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PostsByMarko.Host.Application.Interfaces;
using PostsByMarko.Host.Application.Services;
using PostsByMarko.Host.Data.Entities;
using PostsByMarko.Host.Data.Repositories.EmailOutbox;
using Microsoft.Extensions.Options;
using PostsByMarko.Host.Application.Configuration;

namespace PostsByMarko.UnitTests;

public class EmailOutboxProcessorTests
{
    private readonly Mock<IEmailOutboxRepository> outboxRepositoryMock = new();
    private readonly Mock<IEmailService> emailServiceMock = new();
    private readonly Mock<TimeProvider> timeProviderMock = new();
    private readonly DateTimeOffset now = new(2026, 9, 22, 10, 0, 0, TimeSpan.Zero);
    private readonly EmailConfig emailConfig = new();

    public EmailOutboxProcessorTests()
    {
        timeProviderMock.Setup(provider => provider.GetUtcNow()).Returns(now);
        timeProviderMock.Setup(provider => provider.CreateTimer(
            It.IsAny<TimerCallback>(), It.IsAny<object?>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
            .Returns(Mock.Of<ITimer>());
    }

    [Fact]
    public async Task disabled_delivery_should_leave_queue_untouched_and_resume_when_enabled()
    {
        // Arrange
        var message = new EmailOutboxMessage { Id = Guid.NewGuid(), RecipientEmail = "user@example.com" };
        outboxRepositoryMock.Setup(repository => repository.LeaseNextAsync(
            It.IsAny<string>(), now.UtcDateTime, It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(message);
        emailConfig.Enabled = false;
        var processor = CreateProcessor();

        // Act
        (await processor.ProcessNextAsync()).Should().BeFalse();
        // Assert
        outboxRepositoryMock.VerifyNoOtherCalls();
        emailServiceMock.VerifyNoOtherCalls();

        emailConfig.Enabled = true;
        (await processor.ProcessNextAsync()).Should().BeTrue();
        outboxRepositoryMock.Verify(repository => repository.MarkSentAsync(
            message.Id, It.IsAny<string>(), now.UtcDateTime, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task delivery_timeout_should_cancel_smtp_and_schedule_retry_before_lease_expires()
    {
        // Arrange
        var message = new EmailOutboxMessage { Id = Guid.NewGuid(), RecipientEmail = "user@example.com" };
        outboxRepositoryMock.Setup(repository => repository.LeaseNextAsync(
            It.IsAny<string>(), now.UtcDateTime, TimeSpan.FromMinutes(2), It.IsAny<CancellationToken>()))
            .ReturnsAsync(message);
        TimerCallback? expire = null;
        object? timerState = null;
        timeProviderMock.Setup(provider => provider.CreateTimer(
                It.IsAny<TimerCallback>(), It.IsAny<object?>(), TimeSpan.FromMinutes(1), Timeout.InfiniteTimeSpan))
            .Callback<TimerCallback, object?, TimeSpan, TimeSpan>((callback, state, _, _) =>
            {
                expire = callback;
                timerState = state;
            })
            .Returns(Mock.Of<ITimer>());
        emailServiceMock.Setup(service => service.SendEmailConfirmationLinkAsync(
                message.RecipientEmail, It.IsAny<CancellationToken>()))
            .Returns<string, CancellationToken>(async (_, token) =>
            {
                expire.Should().NotBeNull();
                expire!(timerState);
                await Task.Delay(Timeout.InfiniteTimeSpan, token);
            });

        // Act
        (await CreateProcessor().ProcessNextAsync()).Should().BeTrue();

        // Assert
        outboxRepositoryMock.Verify(repository => repository.MarkFailedAsync(
            message.Id, It.IsAny<string>(), now.UtcDateTime.AddMinutes(1),
            It.Is<string>(error => error.Contains("CanceledException")), It.IsAny<CancellationToken>()), Times.Once);
        outboxRepositoryMock.Verify(repository => repository.MarkSentAsync(
            It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task slow_lease_acquisition_should_not_start_smtp_after_the_delivery_deadline()
    {
        // Arrange
        var message = new EmailOutboxMessage { Id = Guid.NewGuid(), RecipientEmail = "user@example.com" };
        outboxRepositoryMock.Setup(repository => repository.LeaseNextAsync(
                It.IsAny<string>(), now.UtcDateTime, It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(message);
        timeProviderMock.SetupSequence(provider => provider.GetUtcNow())
            .Returns(now).Returns(now.AddSeconds(61)).Returns(now.AddSeconds(61));

        // Act
        (await CreateProcessor().ProcessNextAsync()).Should().BeTrue();

        // Assert
        emailServiceMock.VerifyNoOtherCalls();
        outboxRepositoryMock.Verify(repository => repository.MarkFailedAsync(
            message.Id, It.IsAny<string>(), now.UtcDateTime.AddSeconds(121),
            It.Is<string>(error => error.StartsWith("TimeoutException:")), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task shutdown_cancellation_should_propagate_without_marking_sent_or_failed()
    {
        // Arrange
        using var shutdown = new CancellationTokenSource();
        var message = new EmailOutboxMessage { Id = Guid.NewGuid(), RecipientEmail = "user@example.com" };
        outboxRepositoryMock.Setup(repository => repository.LeaseNextAsync(
                It.IsAny<string>(), now.UtcDateTime, It.IsAny<TimeSpan>(), shutdown.Token))
            .ReturnsAsync(message);
        emailServiceMock.Setup(service => service.SendEmailConfirmationLinkAsync(
                message.RecipientEmail, It.IsAny<CancellationToken>()))
            .Returns<string, CancellationToken>(async (_, token) =>
            {
                shutdown.Cancel();
                await Task.Delay(Timeout.InfiniteTimeSpan, token);
            });

        // Act
        var process = () => CreateProcessor().ProcessNextAsync(shutdown.Token);
        // Assert
        await process.Should().ThrowAsync<OperationCanceledException>();
        outboxRepositoryMock.Verify(repository => repository.MarkSentAsync(
            It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Never);
        outboxRepositoryMock.Verify(repository => repository.MarkFailedAsync(
            It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task process_next_should_send_and_mark_message_as_sent()
    {
        // Arrange
        var message = new EmailOutboxMessage
        {
            Id = Guid.NewGuid(),
            RecipientEmail = "user@example.com"
        };
        timeProviderMock.Setup(provider => provider.GetUtcNow()).Returns(now);
        outboxRepositoryMock.Setup(repository => repository.LeaseNextAsync(
                It.IsAny<string>(), now.UtcDateTime, It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(message);
        var processor = CreateProcessor();

        // Act
        var processed = await processor.ProcessNextAsync();

        // Assert
        processed.Should().BeTrue();
        emailServiceMock.Verify(service => service.SendEmailConfirmationLinkAsync(
            message.RecipientEmail, It.IsAny<CancellationToken>()), Times.Once);
        outboxRepositoryMock.Verify(repository => repository.MarkSentAsync(
            message.Id, It.IsAny<string>(), now.UtcDateTime, It.IsAny<CancellationToken>()), Times.Once);
        outboxRepositoryMock.Verify(repository => repository.MarkFailedAsync(
            It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task process_next_should_schedule_retry_when_delivery_fails()
    {
        // Arrange
        var message = new EmailOutboxMessage
        {
            Id = Guid.NewGuid(),
            RecipientEmail = "user@example.com",
            AttemptCount = 0
        };
        timeProviderMock.Setup(provider => provider.GetUtcNow()).Returns(now);
        outboxRepositoryMock.Setup(repository => repository.LeaseNextAsync(
                It.IsAny<string>(), now.UtcDateTime, It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(message);
        emailServiceMock.Setup(service => service.SendEmailConfirmationLinkAsync(
                message.RecipientEmail, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("SMTP unavailable"));
        var processor = CreateProcessor();

        // Act
        var processed = await processor.ProcessNextAsync();

        // Assert
        processed.Should().BeTrue();
        outboxRepositoryMock.Verify(repository => repository.MarkFailedAsync(
            message.Id,
            It.IsAny<string>(),
            now.UtcDateTime.AddMinutes(1),
            "InvalidOperationException: SMTP unavailable",
            It.IsAny<CancellationToken>()), Times.Once);
        outboxRepositoryMock.Verify(repository => repository.MarkSentAsync(
            It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task process_next_should_return_false_when_queue_is_empty()
    {
        // Arrange
        timeProviderMock.Setup(provider => provider.GetUtcNow()).Returns(now);
        var processor = CreateProcessor();

        // Act
        var processed = await processor.ProcessNextAsync();

        // Assert
        processed.Should().BeFalse();
        emailServiceMock.Verify(service => service.SendEmailConfirmationLinkAsync(
            It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private EmailOutboxProcessor CreateProcessor() => new(
        outboxRepositoryMock.Object,
        emailServiceMock.Object,
        timeProviderMock.Object,
        Options.Create(emailConfig),
        Mock.Of<ILogger<EmailOutboxProcessor>>());
}
