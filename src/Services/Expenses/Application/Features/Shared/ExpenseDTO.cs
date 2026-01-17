namespace Application.Features.Shared;

public record ExpenseDTO
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public double Value { get; init; }
    public DateTime Date { get; init; }
    public string CategoryId { get; init; } = string.Empty;
    public string ApplicationUserI { get; init; } = string.Empty;
};

public sealed record ExtraExpenseDTO : ExpenseDTO
{
    public ApplicationUserDTO ApplicationUser { get; init; } = default!;
    public CategoryDTO Category { get; init; } = default!;
}
