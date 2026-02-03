namespace UnitTests.Features.Commands;

public sealed class DeleteCategoryCommandHandlerTests
{
    private readonly CancellationToken _cancellationToken;
    private readonly Mock<ICategoriesRepository> _repository;
    private readonly Mock<IUnitOfWork> _uow;
    private readonly Mock<IRedisCache> _redisCache;
    private readonly DeleteCategoryCommandHandler _handler;

    public DeleteCategoryCommandHandlerTests()
    {
        _cancellationToken = new();
        _repository = new();
        _uow = new();
        _redisCache = new();

        _handler = new DeleteCategoryCommandHandler(
            _repository.Object,
            _uow.Object,
            _redisCache.Object
        );
    }

    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task DeleteCategoryCommandHandler_WhenCategoryToDeleteNotFound_ShouldReturnNotFoundResultErrorAsync(
        IReadOnlyList<Category> categories
    )
    {
        // Arrange
        var deleteCategoryCommand = new DeleteCategoryCommand(categories[0].Id);
        _repository
            .Setup(mock => mock.DeleteAsync(It.IsAny<string>(), _cancellationToken))
            .ReturnsAsync(
                Result<Empty>.Failure(
                    new Error(
                        Errors.NotFoundError,
                        Errors.NotFoundErrorDetail(nameof(Expense)),
                        ErrorTypes.NotFound
                    )
                )
            )
            .Verifiable();
        _uow.Setup(mock => mock.SaveChangesAsync(_cancellationToken)).Verifiable();
        _redisCache.Setup(mock => mock.RemoveKeysByTags(It.IsAny<string>())).Verifiable();

        // Act
        var result = await _handler.Handle(deleteCategoryCommand, _cancellationToken);

        // Assert
        result
            .Should()
            .BeEquivalentTo(
                Result<Empty>.Failure(
                    new Error(
                        Errors.NotFoundError,
                        Errors.NotFoundErrorDetail(nameof(Expense)),
                        ErrorTypes.NotFound
                    )
                )
            );
        _repository.Verify(
            mock => mock.DeleteAsync(It.IsAny<string>(), _cancellationToken),
            Times.Once
        );
        _uow.Verify(mock => mock.SaveChangesAsync(_cancellationToken), Times.Never);
        _redisCache.Verify(mock => mock.RemoveKeysByTags(It.IsAny<string>()), Times.Never);
    }

    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task DeleteCategoryCommandHandler_WhenExceptionIsThrown_ShouldPropagateException(
        IReadOnlyList<Category> categories
    )
    {
        // Arrange
        var deleteCategoryCommand = new DeleteCategoryCommand(categories[0].Id);
        _repository
            .Setup(mock => mock.DeleteAsync(It.IsAny<string>(), _cancellationToken))
            .ThrowsAsync(new Exception())
            .Verifiable();
        _uow.Setup(mock => mock.SaveChangesAsync(_cancellationToken)).Verifiable();
        _redisCache.Setup(mock => mock.RemoveKeysByTags(It.IsAny<string>())).Verifiable();

        // Act & Assert
        await _handler
            .Invoking(x => x.Handle(deleteCategoryCommand, _cancellationToken))
            .Should()
            .ThrowAsync<Exception>();
        _repository.Verify(
            mock => mock.DeleteAsync(It.IsAny<string>(), _cancellationToken),
            Times.Once
        );
        _uow.Verify(mock => mock.SaveChangesAsync(_cancellationToken), Times.Never);
        _redisCache.Verify(mock => mock.RemoveKeysByTags(It.IsAny<string>()), Times.Never);
    }

    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task DeleteCategoryCommandHandler_WhenValidCommand_ShouldDeleteCategory(
        IReadOnlyList<Category> categories
    )
    {
        // Arrange
        var deleteCategoryCommand = new DeleteCategoryCommand(categories[0].Id);
        _repository
            .Setup(mock => mock.DeleteAsync(It.IsAny<string>(), _cancellationToken))
            .ReturnsAsync(Result<Empty>.Success())
            .Verifiable();
        _uow.Setup(mock => mock.SaveChangesAsync(_cancellationToken)).Verifiable();
        _redisCache.Setup(mock => mock.RemoveKeysByTags(It.IsAny<string>())).Verifiable();

        // Act
        var result = await _handler.Handle(deleteCategoryCommand, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(Result<Empty>.Success());
        _repository.Verify(
            mock => mock.DeleteAsync(It.IsAny<string>(), _cancellationToken),
            Times.Once
        );
        _uow.Verify(mock => mock.SaveChangesAsync(_cancellationToken), Times.Once);
        _redisCache.Verify(mock => mock.RemoveKeysByTags(It.IsAny<string>()), Times.Once);
    }
}
