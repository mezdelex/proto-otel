namespace Application.Features.DomainEvents;

public sealed record PatchedCategoryEvent
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;

    public sealed class PatchedCategoryEventConsumer(ILogger<PatchedCategoryEventConsumer> logger)
        : IConsumer<PatchedCategoryEvent>
    {
        private readonly ILogger<PatchedCategoryEventConsumer> _logger = logger;

        public Task Consume(ConsumeContext<PatchedCategoryEvent> context)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Category patched: {@Category}", context.Message);
            }

            return Task.CompletedTask;
        }
    }
}
