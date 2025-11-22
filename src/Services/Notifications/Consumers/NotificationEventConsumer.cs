namespace Notifications.Consumers;

public sealed class NotificationEventConsumer(ILogger<NotificationEventConsumer> logger)
    : IConsumer<NotificationEvent>
{
    private readonly ILogger<NotificationEventConsumer> _logger = logger;

    public Task Consume(ConsumeContext<NotificationEvent> context)
    {
        _logger.LogInformation(
            "Notification for: {entityType}, {name} and {description}",
            context.Message.EntityType,
            context.Message.Name,
            context.Message.Description
        );

        return Task.CompletedTask;
    }
}
