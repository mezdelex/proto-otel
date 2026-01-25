namespace UnitTests.Mocks;

public static class ExpensesMock
{
    private const int _items = 5;

    public static TheoryData<IReadOnlyList<Expense>> GetExpenses()
    {
        var fixture = new Fixture();
        fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));
        fixture.Customizations.Add(
            new StringPropertyTruncateSpecimenBuilder<Expense>(
                (x => x.Id, ExpenseConstraints.IdMaxLength),
                (x => x.Name, ExpenseConstraints.NameMaxLength),
                (x => x.Description, ExpenseConstraints.DescriptionMaxLength),
                (x => x.CategoryId, CategoryConstraints.IdMaxLength),
                (x => x.ApplicationUserId, ApplicationUserConstraints.IdMaxLength)
            )
        );

        return new TheoryData<IReadOnlyList<Expense>>
        {
            {
                [
                    .. fixture
                        .Build<Expense>()
                        .With(x => x.Category, fixture.Build<Category>().Create())
                        .With(x => x.ApplicationUser, fixture.Build<ApplicationUser>().Create())
                        .CreateMany(_items),
                ]
            },
        };
    }
}
