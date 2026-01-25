namespace UnitTests.Validators.Queries;

public sealed class GetCategoriesQueryValidatorTests()
{
    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task GetCategoriesQueryValidator_WhenNameLengthIsWrong_ShouldReturnValidationError(
        IReadOnlyList<Category> categories
    )
    {
        // Arrange
        var request = new GetCategoriesQuery
        {
            Name = new string('*', CategoryConstraints.NameMaxLength + 1),
            Keyword = categories[0].Name,
        };
        var validator = new GetCategoriesQueryValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(GetCategoriesQuery.Name),
                        CategoryConstraints.NameMaxLength
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task GetCategoriesQueryValidator_WhenKeywordLengthIsWrong_ShouldReturnValidationError(
        IReadOnlyList<Category> categories
    )
    {
        // Arrange
        var request = new GetCategoriesQuery
        {
            Name = categories[0].Name,
            Keyword = new string('*', CategoryConstraints.DescriptionMaxLength + 1),
        };
        var validator = new GetCategoriesQueryValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(GetCategoriesQuery.Keyword),
                        CategoryConstraints.DescriptionMaxLength
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task GetCategoriesQueryValidator_WhenValidQuery_ShouldReturnSuccess()
    {
        // Arrange
        var request = new GetCategoriesQuery();
        var validator = new GetCategoriesQueryValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result.Errors.Should().BeEmpty();
        result.IsValid.Should().BeTrue();
    }
}
