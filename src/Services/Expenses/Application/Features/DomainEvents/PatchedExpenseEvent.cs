namespace Application.Features.DomainEvents;

public sealed record PatchedExpenseEvent
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Value { get; init; }
    public DateTime Date { get; init; }
    public string CategoryId { get; init; } = string.Empty;
    public string ApplicationUserI { get; init; } = string.Empty;

    public sealed class PatchedExpenseEventConsumer(ILogger<PatchedExpenseEventConsumer> logger)
        : IConsumer<PatchedExpenseEvent>
    {
        private readonly ILogger<PatchedExpenseEventConsumer> _logger = logger;

        public Task Consume(ConsumeContext<PatchedExpenseEvent> context)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Expense patched: {@Expense}", context.Message);
            }

            return Task.CompletedTask;
        }
    }
}
