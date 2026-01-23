namespace Domain.Specifications;

public sealed class ExpensesSpecification : Specification<Expense>
{
    public ExpensesSpecification(
        string? id = null,
        string? name = null,
        string? containedWord = null,
        DateTime? minDate = null,
        DateTime? maxDate = null,
        string? categoryId = null,
        string? applicationUserId = null,
        string? email = null,
        Action<ISpecificationBuilder<Expense>>? includes = null
    )
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            Query.Where(x => x.Id.Equals(id));
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            Query.Where(x => x.Name.Equals(name));
        }

        if (!string.IsNullOrWhiteSpace(containedWord))
        {
            Query.Where(x =>
                x.Name.Contains(containedWord) || x.Description.Contains(containedWord)
            );
        }

        /*TODO: add TimeZoneOffset by passing the TimeZone in the request (recommended method)*/
        if (minDate.HasValue)
        {
            Query.Where(x =>
                x.Date.CompareTo(DateTimeNormalizer.NormalizeToUtc(minDate.Value)) >= 0
            );
        }

        if (maxDate.HasValue)
        {
            Query.Where(x =>
                x.Date.CompareTo(DateTimeNormalizer.NormalizeToUtc(maxDate.Value)) <= 0
            );
        }

        if (!string.IsNullOrWhiteSpace(categoryId))
        {
            Query.Where(x => x.CategoryId.Equals(categoryId));
        }

        if (!string.IsNullOrWhiteSpace(applicationUserId))
        {
            Query.Where(x => x.ApplicationUserId.Equals(applicationUserId));
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            Query.Where(x =>
                x.ApplicationUser.Email != null && x.ApplicationUser.Email.Equals(email)
            );
        }

        includes?.Invoke(Query);

        Query.OrderByDescending(x => x.Date).ThenBy(x => x.CategoryId).ThenBy(x => x.Id);
    }
}
