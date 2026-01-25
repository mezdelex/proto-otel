namespace UnitTests.Validators.Queries;

public sealed class GetPaginatedExpensesQueryValidatorTests()
{
    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task GetPaginatedExpensesQueryValidator_WhenNameLengthIsWrong_ShouldReturnValidationError(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new GetPaginatedExpensesQuery
        {
            Name = new string('*', ExpenseConstraints.NameMaxLength + 1),
            Keyword = expenses[0].Name,
            CategoryId = expenses[0].CategoryId,
            ApplicationUserId = expenses[0].ApplicationUserId,
        };
        var validator = new GetPaginatedExpensesQueryValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(GetPaginatedExpensesQuery.Name),
                        ExpenseConstraints.NameMaxLength
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task GetPaginatedExpensesQueryValidator_WhenKeywordLengthIsWrong_ShouldReturnValidationError(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new GetPaginatedExpensesQuery
        {
            Name = expenses[0].Name,
            Keyword = new string('*', ExpenseConstraints.DescriptionMaxLength + 1),
            CategoryId = expenses[0].CategoryId,
            ApplicationUserId = expenses[0].ApplicationUserId,
        };
        var validator = new GetPaginatedExpensesQueryValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(GetPaginatedExpensesQuery.Keyword),
                        ExpenseConstraints.DescriptionMaxLength
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task GetPaginatedExpensesQueryValidator_WhenCategoryIdLengthIsWrong_ShouldReturnValidationError(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new GetPaginatedExpensesQuery
        {
            Name = expenses[0].Name,
            Keyword = expenses[0].Name,
            CategoryId = new string('*', CategoryConstraints.IdMaxLength + 1),
            ApplicationUserId = expenses[0].ApplicationUserId,
        };
        var validator = new GetPaginatedExpensesQueryValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(GetPaginatedExpensesQuery.CategoryId),
                        CategoryConstraints.IdMaxLength
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task GetPaginatedExpensesQueryValidator_WhenApplicationUserIdLengthIsWrong_ShouldReturnValidationError(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new GetPaginatedExpensesQuery
        {
            Name = expenses[0].Name,
            Keyword = expenses[0].Name,
            CategoryId = expenses[0].CategoryId,
            ApplicationUserId = new string('*', ApplicationUserConstraints.IdMaxLength + 1),
        };
        var validator = new GetPaginatedExpensesQueryValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(GetPaginatedExpensesQuery.ApplicationUserId),
                        ApplicationUserConstraints.IdMaxLength
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task GetPaginatedExpensesQueryValidator_WhenValidQuery_ShouldReturnSuccess()
    {
        // Arrange
        var request = new GetPaginatedExpensesQuery();
        var validator = new GetPaginatedExpensesQueryValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result.Errors.Should().BeEmpty();
        result.IsValid.Should().BeTrue();
    }
}
