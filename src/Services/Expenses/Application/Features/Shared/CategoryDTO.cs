namespace Application.Features.Shared;

public record CategoryDTO
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}

public sealed record ExtraCategoryDTO : CategoryDTO
{
    public IEnumerable<ExpenseDTO> Expenses { get; init; } = default!;
}
