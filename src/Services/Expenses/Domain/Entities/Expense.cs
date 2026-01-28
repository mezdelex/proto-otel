namespace Domain.Entities;

public class Expense : AuditEntity, IBaseEntity
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public DateTimeOffset Date { get; set; }
    public string CategoryId { get; set; } = string.Empty;
    public string ApplicationUserId { get; set; } = string.Empty;

    public virtual ApplicationUser ApplicationUser { get; set; } = default!;
    public virtual Category Category { get; set; } = default!;
}

public static class ExpenseConstraints
{
    public const int IdMaxLength = 36;
    public const int NameMaxLength = 32;
    public const int DescriptionMaxLength = 256;
}
