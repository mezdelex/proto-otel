namespace UnitTests.Mocks;

public static class CategoriesMock
{
    private const int _items = 5;

    public static TheoryData<IReadOnlyList<Category>> GetCategories()
    {
        var fixture = new Fixture();
        fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));

        return new TheoryData<IReadOnlyList<Category>>
        {
            { [.. fixture.Build<Category>().CreateMany(_items)] },
        };
    }
}
