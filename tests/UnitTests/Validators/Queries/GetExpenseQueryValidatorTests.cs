namespace UnitTests.Validators.Queries;

public sealed class GetExpenseQueryValidatorTests()
{
    [Fact]
    public async Task GetExpenseQueryValidator_WhenMissingId_ShouldReturnValidationError()
    {
        // Arrange
        var request = new GetExpenseQuery(string.Empty);
        var validator = new GetExpenseQueryValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeEmpty(nameof(GetExpenseQuery.Id))
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task GetExpenseQueryValidator_WhenIdLengthIsWrong_ShouldReturnValidationError()
    {
        // Arrange
        var request = new GetExpenseQuery(new string('*', ExpenseConstraints.IdMaxLength + 1));
        var validator = new GetExpenseQueryValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(GetExpenseQuery.Id),
                        ExpenseConstraints.IdMaxLength
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task GetExpenseQueryValidator_WhenValidQuery_ShouldReturnSuccess(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new GetExpenseQuery(expenses[0].Id);
        var validator = new GetExpenseQueryValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result.Errors.Should().BeEmpty();
        result.IsValid.Should().BeTrue();
    }
}
