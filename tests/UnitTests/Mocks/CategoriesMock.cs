namespace UnitTests.Mocks;

public static class CategoriesMock
{
    private const int _items = 5;

    public static TheoryData<IReadOnlyList<Category>> GetCategories()
    {
        var fixture = new Fixture();
        fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));
        fixture.Customizations.Add(
            new StringPropertyTruncateSpecimenBuilder<Category>(
                (x => x.Id, CategoryConstraints.IdMaxLength),
                (x => x.Name, CategoryConstraints.NameMaxLength),
                (x => x.Description, CategoryConstraints.DescriptionMaxLength)
            )
        );

        return new TheoryData<IReadOnlyList<Category>>
        {
            { [.. fixture.Build<Category>().CreateMany(_items)] },
        };
    }
}
