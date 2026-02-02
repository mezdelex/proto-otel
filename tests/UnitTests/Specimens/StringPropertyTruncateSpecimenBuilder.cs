namespace UnitTests.Specimens;

public class StringPropertyTruncateSpecimenBuilder<TEntity>(
    params (Expression<Func<TEntity, string>> Getter, int Length)[] constraints
) : ISpecimenBuilder
{
    private readonly Dictionary<string, int> _propertyLengths = constraints.ToDictionary(
        x => GetPropertyInfo(x.Getter).Name,
        x => x.Length
    );

    public object Create(object request, ISpecimenContext context)
    {
        if (
            request is PropertyInfo propertyInfo
            && propertyInfo.DeclaringType == typeof(TEntity)
            && _propertyLengths.TryGetValue(propertyInfo.Name, out var maxLength)
        )
        {
            var value = context.Create<string>();

            return value.Length <= maxLength ? value : value[..maxLength];
        }

        return new NoSpecimen();
    }

    private static PropertyInfo GetPropertyInfo(Expression<Func<TEntity, string>> expression)
    {
        if (expression.Body is MemberExpression member && member.Member is PropertyInfo prop)
        {
            return prop;
        }

        throw new ArgumentException("Expression must be a property getter.");
    }
}
