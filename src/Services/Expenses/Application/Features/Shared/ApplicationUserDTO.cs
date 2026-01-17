namespace Application.Features.Shared;

public record ApplicationUserDTO
{
    public string Id { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}

public sealed record ExtraApplicationUserDTO : ApplicationUserDTO
{
    public IEnumerable<ExpenseDTO> Expenses { get; init; } = default!;
}
