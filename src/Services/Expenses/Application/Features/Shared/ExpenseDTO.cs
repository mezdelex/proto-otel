namespace Application.Features.Shared;

public record ExpenseDTO
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Value { get; init; }
    public DateTimeOffset Date { get; init; }
    public string CategoryId { get; init; } = string.Empty;
    public string ApplicationUserId { get; init; } = string.Empty;
};

public sealed record ExtraExpenseDTO : ExpenseDTO
{
    public CategoryDTO Category { get; init; } = default!;
    public ApplicationUserDTO ApplicationUser { get; init; } = default!;
}
