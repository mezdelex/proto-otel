namespace UnitTests.Validators.Commands;

public sealed class DeleteCategoryCommandValidatorTests()
{
    [Fact]
    public async Task DeleteCategoryCommandValidator_WhenMissingId_ShouldReturnValidationError()
    {
        // Arrange
        var request = new DeleteCategoryCommand(string.Empty);
        var validator = new DeleteCategoryCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeEmpty(nameof(DeleteCategoryCommand.Id))
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteCategoryCommandValidator_WhenIdLengthIsWrong_ShouldReturnValidationError()
    {
        // Arrange
        var request = new DeleteCategoryCommand(
            new string('*', CategoryConstraints.IdMaxLength + 1)
        );
        var validator = new DeleteCategoryCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(DeleteCategoryCommand.Id),
                        CategoryConstraints.IdMaxLength
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task DeleteCategoryCommandValidator_WhenValidCommand_ShouldReturnValidationSuccess(
        IReadOnlyList<Category> categories
    )
    {
        // Arrange
        var request = new DeleteCategoryCommand(categories[0].Id);
        var validator = new DeleteCategoryCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result.Errors.Should().BeEmpty();
        result.IsValid.Should().BeTrue();
    }
}
