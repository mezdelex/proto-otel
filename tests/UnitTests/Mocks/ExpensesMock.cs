namespace UnitTests.Mocks;

public static class ExpensesMock
{
    private const int _items = 5;

    public static TheoryData<
        IReadOnlyList<Expense>,
        IReadOnlyList<ApplicationUser>
    > GetExpensesWithUsers()
    {
        var fixture = new Fixture();
        fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));

        return new TheoryData<IReadOnlyList<Expense>, IReadOnlyList<ApplicationUser>>
        {
            {
                [.. fixture.Build<Expense>().CreateMany(_items)],
                [.. fixture.Build<ApplicationUser>().CreateMany(_items)]
            },
        };
    }
}
