namespace Domain.Entities;

public interface IBaseEntity
{
    public string Id { get; set; }
}

public class BaseConstraints
{
    public const int DatePrecision = 3;
    public const int DecimalPrecision = 19;
    public const int DecimalScale = 4;
}
