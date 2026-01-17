namespace Application.Features.DomainEvents;

public sealed record PostedCategoryEvent
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;

    public sealed class PostedCategoryEventConsumer(ILogger<PostedCategoryEventConsumer> logger)
        : IConsumer<PostedCategoryEvent>
    {
        private readonly ILogger<PostedCategoryEventConsumer> _logger = logger;

        public Task Consume(ConsumeContext<PostedCategoryEvent> context)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Category created: {@Category}", context.Message);
            }

            return Task.CompletedTask;
        }
    }
}
