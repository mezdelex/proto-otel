namespace Domain.Converters;

public static class DateTimeNormalizer
{
    public static DateTime NormalizeToUtc(DateTime date) =>
        date.Kind == DateTimeKind.Utc ? date : date.ToUniversalTime();
}
