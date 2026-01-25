namespace UnitTests.Validators.Commands;

public sealed class PatchCategoryCommandValidatorTests()
{
    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task PatchCategoryCommandValidator_WhenMissingId_ShouldReturnValidationError(
        IReadOnlyList<Category> categories
    )
    {
        // Arrange
        var request = new PatchCategoryCommand(
            string.Empty,
            categories[0].Name,
            categories[0].Description
        );
        var validator = new PatchCategoryCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeEmpty(nameof(PatchCategoryCommand.Id))
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task PatchCategoryCommandValidator_WhenIdLengthIsWrong_ShouldReturnValidationError(
        IReadOnlyList<Category> categories
    )
    {
        // Arrange
        var request = new PatchCategoryCommand(
            new string('*', CategoryConstraints.IdMaxLength + 1),
            categories[0].Name,
            categories[0].Description
        );
        var validator = new PatchCategoryCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(PatchCategoryCommand.Id),
                        CategoryConstraints.IdMaxLength
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task PatchCategoryCommandValidator_WhenMissingName_ShouldReturnValidationError(
        IReadOnlyList<Category> categories
    )
    {
        // Arrange
        var request = new PatchCategoryCommand(
            categories[0].Id,
            string.Empty,
            categories[0].Description
        );
        var validator = new PatchCategoryCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeEmpty(nameof(PatchCategoryCommand.Name))
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task PatchCategoryCommandValidator_WhenNameLengthIsWrong_ShouldReturnValidationError(
        IReadOnlyList<Category> categories
    )
    {
        // Arrange
        var request = new PatchCategoryCommand(
            categories[0].Id,
            new string('*', CategoryConstraints.NameMaxLength + 1),
            categories[0].Description
        );
        var validator = new PatchCategoryCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(PatchCategoryCommand.Name),
                        CategoryConstraints.NameMaxLength
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task PatchCategoryCommandValidator_WhenMissingDescription_ShouldReturnValidationError(
        IReadOnlyList<Category> categories
    )
    {
        // Arrange
        var request = new PatchCategoryCommand(categories[0].Id, categories[0].Name, string.Empty);
        var validator = new PatchCategoryCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeEmpty(
                        nameof(PatchCategoryCommand.Description)
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task PatchCategoryCommandValidator_WhenDescriptionLengthIsWrong_ShouldReturnValidationError(
        IReadOnlyList<Category> categories
    )
    {
        // Arrange
        var request = new PatchCategoryCommand(
            categories[0].Id,
            categories[0].Name,
            new string('*', CategoryConstraints.DescriptionMaxLength + 1)
        );
        var validator = new PatchCategoryCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(PatchCategoryCommand.Description),
                        CategoryConstraints.DescriptionMaxLength
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task PatchCategoryCommandValidator_WhenValidCommand_ShouldReturnValidationSuccess(
        IReadOnlyList<Category> categories
    )
    {
        // Arrange
        var request = new PatchCategoryCommand(
            categories[0].Id,
            categories[0].Name,
            categories[0].Description
        );
        var validator = new PatchCategoryCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result.Errors.Should().BeEmpty();
        result.IsValid.Should().BeTrue();
    }
}
