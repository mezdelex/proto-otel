namespace UnitTests.Validators.Commands;

public sealed class PostExpenseCommandValidatorTests()
{
    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task PostExpenseCommandValidator_WhenMissingName_ShouldReturnValidationError(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new PostExpenseCommand(
            string.Empty,
            expenses[0].Description,
            expenses[0].Value,
            expenses[0].CategoryId
        );
        var validator = new PostExpenseCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeEmpty(nameof(PostExpenseCommand.Name))
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task PostExpenseCommandValidator_WhenNameLengthIsWrong_ShouldReturnValidationError(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new PostExpenseCommand(
            new string('*', ExpenseConstraints.NameMaxLength + 1),
            expenses[0].Description,
            expenses[0].Value,
            expenses[0].CategoryId
        );
        var validator = new PostExpenseCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(PostExpenseCommand.Name),
                        ExpenseConstraints.NameMaxLength
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task PostExpenseCommandValidator_WhenMissingDescription_ShouldReturnValidationError(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new PostExpenseCommand(
            expenses[0].Name,
            string.Empty,
            expenses[0].Value,
            expenses[0].CategoryId
        );
        var validator = new PostExpenseCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeEmpty(
                        nameof(PostExpenseCommand.Description)
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task PostExpenseCommandValidator_WhenDescriptionLengthIsWrong_ShouldReturnValidationError(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new PostExpenseCommand(
            expenses[0].Name,
            new string('*', ExpenseConstraints.DescriptionMaxLength + 1),
            expenses[0].Value,
            expenses[0].CategoryId
        );
        var validator = new PostExpenseCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(PostExpenseCommand.Description),
                        ExpenseConstraints.DescriptionMaxLength
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task PostExpenseCommandValidator_WhenMissingValue_ShouldReturnValidationError(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new PostExpenseCommand(
            expenses[0].Name,
            expenses[0].Description,
            default,
            expenses[0].CategoryId
        );
        var validator = new PostExpenseCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeEmpty(nameof(PostExpenseCommand.Value))
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task PostExpenseCommandValidator_WhenMissingCategoryId_ShouldReturnValidationError(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new PostExpenseCommand(
            expenses[0].Name,
            expenses[0].Description,
            expenses[0].Value,
            string.Empty
        );
        var validator = new PostExpenseCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeEmpty(
                        nameof(PostExpenseCommand.CategoryId)
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task PostExpenseCommandValidator_WhenCategoryIdLengthIsWrong_ShouldReturnValidationError(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new PostExpenseCommand(
            expenses[0].Name,
            expenses[0].Description,
            expenses[0].Value,
            new string('*', CategoryConstraints.IdMaxLength + 1)
        );
        var validator = new PostExpenseCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(PostExpenseCommand.CategoryId),
                        CategoryConstraints.IdMaxLength
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task PostExpenseCommandValidator_WhenValidCommand_ShouldReturnValidationSuccess(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new PostExpenseCommand(
            expenses[0].Name,
            expenses[0].Description,
            expenses[0].Value,
            expenses[0].CategoryId
        );
        var validator = new PostExpenseCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result.Errors.Should().BeEmpty();
        result.IsValid.Should().BeTrue();
    }
}
