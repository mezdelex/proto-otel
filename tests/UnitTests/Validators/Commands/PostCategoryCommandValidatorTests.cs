namespace UnitTests.Validators.Commands;

public sealed class PostCategoryCommandValidatorTests()
{
    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task PostCategoryCommandValidator_WhenMissingName_ShouldReturnValidationError(
        IReadOnlyList<Category> categories
    )
    {
        // Arrange
        var request = new PostCategoryCommand(string.Empty, categories[0].Description);
        var validator = new PostCategoryCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeEmpty(nameof(PostCategoryCommand.Name))
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task PostCategoryCommandValidator_WhenNameLengthIsWrong_ShouldReturnValidationError(
        IReadOnlyList<Category> categories
    )
    {
        // Arrange
        var request = new PostCategoryCommand(
            new string('*', CategoryConstraints.NameMaxLength + 1),
            categories[0].Description
        );
        var validator = new PostCategoryCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(PostCategoryCommand.Name),
                        CategoryConstraints.NameMaxLength
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task PostCategoryCommandValidator_WhenMissingDescription_ShouldReturnValidationError(
        IReadOnlyList<Category> categories
    )
    {
        // Arrange
        var request = new PostCategoryCommand(categories[0].Name, string.Empty);
        var validator = new PostCategoryCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeEmpty(
                        nameof(PostCategoryCommand.Description)
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task PostCategoryCommandValidator_WhenDescriptionLengthIsWrong_ShouldReturnValidationError(
        IReadOnlyList<Category> categories
    )
    {
        // Arrange
        var request = new PostCategoryCommand(
            categories[0].Name,
            new string('*', CategoryConstraints.DescriptionMaxLength + 1)
        );
        var validator = new PostCategoryCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(PostCategoryCommand.Description),
                        CategoryConstraints.DescriptionMaxLength
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task PostCategoryCommandValidator_WhenValidCommand_ShouldReturnValidationSuccess(
        IReadOnlyList<Category> categories
    )
    {
        // Arrange
        var request = new PostCategoryCommand(categories[0].Name, categories[0].Description);
        var validator = new PostCategoryCommandValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result.Errors.Should().BeEmpty();
        result.IsValid.Should().BeTrue();
    }
}
