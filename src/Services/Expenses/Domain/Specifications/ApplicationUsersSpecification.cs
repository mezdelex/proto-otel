namespace Domain.Specifications;

public sealed class ApplicationUsersSpecification : Specification<ApplicationUser>
{
    public ApplicationUsersSpecification(
        string? id = null,
        string? email = null,
        string? containedWord = null,
        Action<ISpecificationBuilder<ApplicationUser>>? includes = null
    )
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            Query.Where(x => x.Id.Equals(id));
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            Query.Where(x => x.Email != null && x.Email.Equals(email));
        }

        if (!string.IsNullOrWhiteSpace(containedWord))
        {
            Query.Where(x =>
                x.Email != null && x.Email.Contains(containedWord)
                || x.UserName != null && x.UserName.Contains(containedWord)
            );
        }

        includes?.Invoke(Query);

        Query.OrderBy(x => x.Email).ThenBy(x => x.Id);
    }
}
