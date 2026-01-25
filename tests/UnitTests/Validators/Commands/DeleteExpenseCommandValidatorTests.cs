namespace UnitTests.Validators.Commands;

public sealed class DeleteExpenseCommandValidatorTests()
{
    [Fact]
    public async Task DeleteExpenseCommandValidator_WhenMissingId_ShouldReturnValidationError()
    {
        // Arrange
        var request = new DeleteExpenseCommand(string.Empty);
        var validator = new DeleteExpenseCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeEmpty(nameof(DeleteExpenseCommand.Id))
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteExpenseCommandValidator_WhenIdLengthIsWrong_ShouldReturnValidationError()
    {
        // Arrange
        var request = new DeleteExpenseCommand(new string('*', ExpenseConstraints.IdMaxLength + 1));
        var validator = new DeleteExpenseCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(DeleteExpenseCommand.Id),
                        ExpenseConstraints.IdMaxLength
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task DeleteExpenseCommandValidator_WhenValidCommand_ShouldReturnValidationSuccess(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new DeleteExpenseCommand(expenses[0].Id);
        var validator = new DeleteExpenseCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result.Errors.Should().BeEmpty();
        result.IsValid.Should().BeTrue();
    }
}
