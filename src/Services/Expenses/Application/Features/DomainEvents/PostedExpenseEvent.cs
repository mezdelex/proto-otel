namespace Application.Features.DomainEvents;

public record PostedExpenseEvent
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Value { get; init; }
    public DateTime Date { get; init; }
    public string CategoryId { get; init; } = string.Empty;
    public string ApplicationUserI { get; init; } = string.Empty;

    public sealed class PostedExpenseEventConsumer(ILogger<PostedExpenseEventConsumer> logger)
        : IConsumer<PostedExpenseEvent>
    {
        private readonly ILogger<PostedExpenseEventConsumer> _logger = logger;

        public Task Consume(ConsumeContext<PostedExpenseEvent> context)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Expense posted: {@Expense}", context.Message);
            }

            return Task.CompletedTask;
        }
    }
}
