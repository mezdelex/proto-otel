namespace UnitTests.Validators.Queries;

public sealed class GetPaginatedCategoriesQueryValidatorTests()
{
    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task GetPaginatedCategoriesQueryValidator_WhenNameLengthIsWrong_ShouldReturnValidationError(
        IReadOnlyList<Category> categories
    )
    {
        // Arrange
        var request = new GetPaginatedCategoriesQuery
        {
            Name = new string('*', CategoryConstraints.NameMaxLength + 1),
            Keyword = categories[0].Name,
        };
        var validator = new GetPaginatedCategoriesQueryValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(GetPaginatedCategoriesQuery.Name),
                        CategoryConstraints.NameMaxLength
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task GetPaginatedCategoriesQueryValidator_WhenKeywordLengthIsWrong_ShouldReturnValidationError(
        IReadOnlyList<Category> categories
    )
    {
        // Arrange
        var request = new GetPaginatedCategoriesQuery
        {
            Name = categories[0].Name,
            Keyword = new string('*', CategoryConstraints.DescriptionMaxLength + 1),
        };
        var validator = new GetPaginatedCategoriesQueryValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(GetPaginatedCategoriesQuery.Keyword),
                        CategoryConstraints.DescriptionMaxLength
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task GetPaginatedCategoriesQueryValidator_WhenValidQuery_ShouldReturnSuccess()
    {
        // Arrange
        var request = new GetPaginatedCategoriesQuery();
        var validator = new GetPaginatedCategoriesQueryValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result.Errors.Should().BeEmpty();
        result.IsValid.Should().BeTrue();
    }
}
