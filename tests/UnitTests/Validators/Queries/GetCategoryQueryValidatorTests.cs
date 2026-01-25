namespace UnitTests.Validators.Queries;

public sealed class GetCategoryQueryValidatorTests()
{
    [Fact]
    public async Task GetCategoryQueryValidator_WhenMissingId_ShouldReturnValidationError()
    {
        // Arrange
        var request = new GetCategoryQuery(string.Empty);
        var validator = new GetCategoryQueryValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeEmpty(nameof(GetCategoryQuery.Id))
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task GetCategoryQueryValidator_WhenIdLengthIsWrong_ShouldReturnValidationError()
    {
        // Arrange
        var request = new GetCategoryQuery(new string('*', CategoryConstraints.IdMaxLength + 1));
        var validator = new GetCategoryQueryValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result
            .Errors.Should()
            .ContainSingle(x =>
                x.ErrorMessage.Equals(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(GetCategoryQuery.Id),
                        CategoryConstraints.IdMaxLength
                    )
                )
            );
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task GetCategoryQueryValidator_WhenValidQuery_ShouldReturnSuccess(
        IReadOnlyList<Category> categories
    )
    {
        // Arrange
        var request = new GetCategoryQuery(categories[0].Id);
        var validator = new GetCategoryQueryValidator();

        // Act
        var result = await validator.TestValidateAsync(request);

        // Assert
        result.Errors.Should().BeEmpty();
        result.IsValid.Should().BeTrue();
    }
}
