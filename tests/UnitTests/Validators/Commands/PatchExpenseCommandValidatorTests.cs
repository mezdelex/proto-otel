namespace UnitTests.Validators.Commands;

public sealed class PatchExpenseCommandValidatorTests()
{
    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task PatchExpenseCommandValidator_WhenMissingId_ShouldReturnValidationError(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new PatchExpenseCommand(
            string.Empty,
            expenses[0].Name,
            expenses[0].Description,
            expenses[0].Value,
            expenses[0].Date,
            expenses[0].CategoryId,
            expenses[0].ApplicationUserId
        );
        var validator = new PatchExpenseCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeEmpty(nameof(PatchExpenseCommand.Id))
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task PatchExpenseCommandValidator_WhenIdLengthIsWrong_ShouldReturnValidationError(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new PatchExpenseCommand(
            new string('*', ExpenseConstraints.IdMaxLength + 1),
            expenses[0].Name,
            expenses[0].Description,
            expenses[0].Value,
            expenses[0].Date,
            expenses[0].CategoryId,
            expenses[0].ApplicationUserId
        );
        var validator = new PatchExpenseCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(PatchExpenseCommand.Id),
                        ExpenseConstraints.IdMaxLength
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task PatchExpenseCommandValidator_WhenMissingName_ShouldReturnValidationError(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new PatchExpenseCommand(
            expenses[0].Id,
            string.Empty,
            expenses[0].Description,
            expenses[0].Value,
            expenses[0].Date,
            expenses[0].CategoryId,
            expenses[0].ApplicationUserId
        );
        var validator = new PatchExpenseCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeEmpty(nameof(PatchExpenseCommand.Name))
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task PatchExpenseCommandValidator_WhenNameLengthIsWrong_ShouldReturnValidationError(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new PatchExpenseCommand(
            expenses[0].Id,
            new string('*', ExpenseConstraints.NameMaxLength + 1),
            expenses[0].Description,
            expenses[0].Value,
            expenses[0].Date,
            expenses[0].CategoryId,
            expenses[0].ApplicationUserId
        );
        var validator = new PatchExpenseCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(PatchExpenseCommand.Name),
                        ExpenseConstraints.NameMaxLength
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task PatchExpenseCommandValidator_WhenMissingDescription_ShouldReturnValidationError(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new PatchExpenseCommand(
            expenses[0].Id,
            expenses[0].Name,
            string.Empty,
            expenses[0].Value,
            expenses[0].Date,
            expenses[0].CategoryId,
            expenses[0].ApplicationUserId
        );
        var validator = new PatchExpenseCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeEmpty(
                        nameof(PatchExpenseCommand.Description)
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task PatchExpenseCommandValidator_WhenDescriptionLengthIsWrong_ShouldReturnValidationError(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new PatchExpenseCommand(
            expenses[0].Id,
            expenses[0].Name,
            new string('*', ExpenseConstraints.DescriptionMaxLength + 1),
            expenses[0].Value,
            expenses[0].Date,
            expenses[0].CategoryId,
            expenses[0].ApplicationUserId
        );
        var validator = new PatchExpenseCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(PatchExpenseCommand.Description),
                        ExpenseConstraints.DescriptionMaxLength
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task PatchExpenseCommandValidator_WhenMissingValue_ShouldReturnValidationError(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new PatchExpenseCommand(
            expenses[0].Id,
            expenses[0].Name,
            expenses[0].Description,
            default,
            expenses[0].Date,
            expenses[0].CategoryId,
            expenses[0].ApplicationUserId
        );
        var validator = new PatchExpenseCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeEmpty(nameof(PatchExpenseCommand.Value))
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task PatchExpenseCommandValidator_WhenMissingDate_ShouldReturnValidationError(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new PatchExpenseCommand(
            expenses[0].Id,
            expenses[0].Name,
            expenses[0].Description,
            expenses[0].Value,
            default,
            expenses[0].CategoryId,
            expenses[0].ApplicationUserId
        );
        var validator = new PatchExpenseCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeEmpty(nameof(PatchExpenseCommand.Date))
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task PatchExpenseCommandValidator_WhenMissingCategoryId_ShouldReturnValidationError(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new PatchExpenseCommand(
            expenses[0].Id,
            expenses[0].Name,
            expenses[0].Description,
            expenses[0].Value,
            expenses[0].Date,
            string.Empty,
            expenses[0].ApplicationUserId
        );
        var validator = new PatchExpenseCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeEmpty(
                        nameof(PatchExpenseCommand.CategoryId)
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task PatchExpenseCommandValidator_WhenCategoryIdLengthIsWrong_ShouldReturnValidationError(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new PatchExpenseCommand(
            expenses[0].Id,
            expenses[0].Name,
            expenses[0].Description,
            expenses[0].Value,
            expenses[0].Date,
            new string('*', CategoryConstraints.IdMaxLength + 1),
            expenses[0].ApplicationUserId
        );
        var validator = new PatchExpenseCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(PatchExpenseCommand.CategoryId),
                        CategoryConstraints.IdMaxLength
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task PatchExpenseCommandValidator_WhenMissingApplicationUserId_ShouldReturnValidationError(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new PatchExpenseCommand(
            expenses[0].Id,
            expenses[0].Name,
            expenses[0].Description,
            expenses[0].Value,
            expenses[0].Date,
            expenses[0].CategoryId,
            string.Empty
        );
        var validator = new PatchExpenseCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeEmpty(
                        nameof(PatchExpenseCommand.ApplicationUserId)
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task PatchExpenseCommandValidator_WhenApplicationUserIdLengthIsWrong_ShouldReturnValidationError(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new PatchExpenseCommand(
            expenses[0].Id,
            expenses[0].Name,
            expenses[0].Description,
            expenses[0].Value,
            expenses[0].Date,
            expenses[0].CategoryId,
            new string('*', ApplicationUserConstraints.IdMaxLength + 1)
        );
        var validator = new PatchExpenseCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(PatchExpenseCommand.ApplicationUserId),
                        ApplicationUserConstraints.IdMaxLength
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task PatchExpenseCommandValidator_WhenValidCommand_ShouldReturnValidationSuccess(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new PatchExpenseCommand(
            expenses[0].Id,
            expenses[0].Name,
            expenses[0].Description,
            expenses[0].Value,
            expenses[0].Date,
            expenses[0].CategoryId,
            expenses[0].ApplicationUserId
        );
        var validator = new PatchExpenseCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result.Errors.Should().BeEmpty();
        result.IsValid.Should().BeTrue();
    }
}
