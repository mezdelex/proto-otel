namespace UnitTests.Features.Commands;

public sealed class PatchCategoryCommandHandlerTests
{
    private readonly CancellationToken _cancellationToken;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IMapper _mapper;
    private readonly Mock<ICategoriesRepository> _repository;
    private readonly Mock<IUnitOfWork> _uow;
    private readonly Mock<IRedisCache> _redisCache;
    private readonly Mock<IEventBus> _eventBus;
    private readonly PatchCategoryCommandHandler _handler;

    public PatchCategoryCommandHandlerTests()
    {
        _cancellationToken = new();
        _loggerFactory = new LoggerFactory();
        _mapper = new MapperConfiguration(
            c => c.AddProfile<CategoriesProfile>(),
            _loggerFactory
        ).CreateMapper();
        _repository = new();
        _uow = new();
        _redisCache = new();
        _eventBus = new();

        _handler = new PatchCategoryCommandHandler(
            _mapper,
            _repository.Object,
            _uow.Object,
            _redisCache.Object,
            _eventBus.Object
        );
    }

    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task PatchCategoryCommandHandler_WhenCategoryToPatchNotFound_ShouldReturnNotFoundResultErrorAsync(
        IReadOnlyList<Category> categories
    )
    {
        // Arrange
        var request = new PatchCategoryCommand(
            categories[0].Id,
            categories[0].Name,
            categories[0].Description
        );
        _repository
            .Setup(mock => mock.PatchAsync(It.IsAny<Category>(), _cancellationToken))
            .ReturnsAsync(
                Result<Empty>.Failure(
                    new Error(
                        Errors.NotFoundError,
                        Errors.NotFoundErrorDetail(nameof(Category)),
                        ErrorTypes.NotFound
                    )
                )
            )
            .Verifiable();
        _uow.Setup(mock => mock.SaveChangesAsync(_cancellationToken)).Verifiable();
        _redisCache.Setup(mock => mock.RemoveKeysByTags(It.IsAny<string>())).Verifiable();
        _eventBus
            .Setup(mock => mock.PublishAsync(It.IsAny<PatchedCategoryEvent>(), _cancellationToken))
            .Verifiable();

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result
            .Should()
            .BeEquivalentTo(
                Result<Empty>.Failure(
                    new Error(
                        Errors.NotFoundError,
                        Errors.NotFoundErrorDetail(nameof(Category)),
                        ErrorTypes.NotFound
                    )
                )
            );
        _repository.Verify(
            mock => mock.PatchAsync(It.IsAny<Category>(), _cancellationToken),
            Times.Once
        );
        _uow.Verify(mock => mock.SaveChangesAsync(_cancellationToken), Times.Never);
        _redisCache.Verify(mock => mock.RemoveKeysByTags(It.IsAny<string>()), Times.Never);
        _eventBus.Verify(
            mock => mock.PublishAsync(It.IsAny<PatchedCategoryEvent>(), _cancellationToken),
            Times.Never
        );
    }

    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task PatchCategoryCommandHandler_WhenExceptionIsThrown_ShouldPropagateException(
        IReadOnlyList<Category> categories
    )
    {
        // Arrange
        var request = new PatchCategoryCommand(
            categories[0].Id,
            categories[0].Name,
            categories[0].Description
        );
        _repository
            .Setup(mock => mock.PatchAsync(It.IsAny<Category>(), _cancellationToken))
            .ThrowsAsync(new Exception())
            .Verifiable();
        _uow.Setup(mock => mock.SaveChangesAsync(_cancellationToken)).Verifiable();
        _redisCache.Setup(mock => mock.RemoveKeysByTags(It.IsAny<string>())).Verifiable();
        _eventBus
            .Setup(mock => mock.PublishAsync(It.IsAny<PatchedCategoryEvent>(), _cancellationToken))
            .Verifiable();

        // Act & Assert
        await _handler
            .Invoking(x => x.Handle(request, _cancellationToken))
            .Should()
            .ThrowAsync<Exception>();
        _repository.Verify(
            mock => mock.PatchAsync(It.IsAny<Category>(), _cancellationToken),
            Times.Once
        );
        _uow.Verify(mock => mock.SaveChangesAsync(_cancellationToken), Times.Never);
        _redisCache.Verify(mock => mock.RemoveKeysByTags(It.IsAny<string>()), Times.Never);
        _eventBus.Verify(
            mock => mock.PublishAsync(It.IsAny<PatchedCategoryEvent>(), _cancellationToken),
            Times.Never
        );
    }

    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task PatchCategoryCommandHandler_WhenValidQuery_ShouldPatchCategoryAndPublishEventAsync(
        IReadOnlyList<Category> categories
    )
    {
        // Arrange
        var request = new PatchCategoryCommand(
            categories[0].Id,
            categories[0].Name,
            categories[0].Description
        );
        _repository
            .Setup(mock => mock.PatchAsync(It.IsAny<Category>(), _cancellationToken))
            .ReturnsAsync(Result<Empty>.Success())
            .Verifiable();
        _uow.Setup(mock => mock.SaveChangesAsync(_cancellationToken)).Verifiable();
        _redisCache.Setup(mock => mock.RemoveKeysByTags(It.IsAny<string>())).Verifiable();
        _eventBus
            .Setup(mock => mock.PublishAsync(It.IsAny<PatchedCategoryEvent>(), _cancellationToken))
            .Verifiable();

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(Result<Empty>.Success());
        _repository.Verify(
            mock => mock.PatchAsync(It.IsAny<Category>(), _cancellationToken),
            Times.Once
        );
        _uow.Verify(mock => mock.SaveChangesAsync(_cancellationToken), Times.Once);
        _redisCache.Verify(mock => mock.RemoveKeysByTags(It.IsAny<string>()), Times.Once());
        _eventBus.Verify(
            mock => mock.PublishAsync(It.IsAny<PatchedCategoryEvent>(), _cancellationToken),
            Times.Once
        );
    }
}
