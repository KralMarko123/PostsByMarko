using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PostsByMarko.Host.Application.Interfaces;
using PostsByMarko.Host.Application.Services;
using PostsByMarko.Host.Data.Entities;
using PostsByMarko.Host.Data.Repositories.EmailOutbox;

namespace PostsByMarko.UnitTests;

public class EmailOutboxProcessorTests
{
    private readonly Mock<IEmailOutboxRepository> outboxRepositoryMock = new();
    private readonly Mock<IEmailService> emailServiceMock = new();
    private readonly Mock<TimeProvider> timeProviderMock = new();
    private readonly DateTimeOffset now = new(2026, 9, 22, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task process_next_should_send_and_mark_message_as_sent()
    {
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

        var processed = await processor.ProcessNextAsync();

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

        var processed = await processor.ProcessNextAsync();

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
        timeProviderMock.Setup(provider => provider.GetUtcNow()).Returns(now);
        var processor = CreateProcessor();

        var processed = await processor.ProcessNextAsync();

        processed.Should().BeFalse();
        emailServiceMock.Verify(service => service.SendEmailConfirmationLinkAsync(
            It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private EmailOutboxProcessor CreateProcessor() => new(
        outboxRepositoryMock.Object,
        emailServiceMock.Object,
        timeProviderMock.Object,
        Mock.Of<ILogger<EmailOutboxProcessor>>());
}
