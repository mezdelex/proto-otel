namespace UnitTests.Features.Queries;

public sealed class GetCategoriesQueryHandlerTests
{
    private readonly CancellationToken _cancellationToken;
    private readonly Mock<IRedisCache> _redisCache;
    private readonly Mock<ICategoriesRepository> _repository;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IMapper _mapper;
    private readonly GetCategoriesQueryHandler _handler;

    public GetCategoriesQueryHandlerTests()
    {
        _cancellationToken = new();
        _redisCache = new();
        _repository = new();
        _loggerFactory = new LoggerFactory();
        _mapper = new MapperConfiguration(
            c =>
            {
                c.AddProfile<CategoriesProfile>();
                c.AddProfile<ExpensesProfile>();
            },
            _loggerFactory
        ).CreateMapper();

        _handler = new GetCategoriesQueryHandler(_redisCache.Object, _repository.Object, _mapper);
    }

    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task GetCategoriesQueryHandler_WhenExceptionIsThrown_ShouldPropagateException(
        IReadOnlyList<Category> categories
    )
    {
        // Arrange
        var request = new GetCategoriesQuery
        {
            Name = categories[0].Name,
            Keyword = categories[0].Name,
        };
        var redisKey = $"TestKey";
        _redisCache.Setup(mock => mock.GenerateKey(It.IsAny<object?[]>())).Returns(redisKey);
        _redisCache
            .Setup(mock => mock.GetCachedData<List<CategoryDTO>>(redisKey))
            .ReturnsAsync((List<CategoryDTO>)null!);
        _repository
            .Setup(mock => mock.ApplySpecification(It.IsAny<CategoriesSpecification>()))
            .Throws(new Exception())
            .Verifiable();
        _redisCache
            .Setup(mock =>
                mock.SetCachedData(
                    redisKey,
                    It.IsAny<List<CategoryDTO>>(),
                    It.IsAny<TimeSpan>(),
                    It.IsAny<string[]>()
                )
            )
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act & Assert
        await _handler
            .Invoking(x => x.Handle(request, _cancellationToken))
            .Should()
            .ThrowAsync<Exception>();
        _redisCache.Verify(mock => mock.GenerateKey(It.IsAny<object?[]>()), Times.Once());
        _redisCache.Verify(mock => mock.GetCachedData<List<CategoryDTO>>(redisKey), Times.Once);
        _repository.Verify(
            mock => mock.ApplySpecification(It.IsAny<CategoriesSpecification>()),
            Times.Once
        );
        _redisCache.Verify(
            mock =>
                mock.SetCachedData(
                    redisKey,
                    It.IsAny<List<CategoryDTO>>(),
                    It.IsAny<TimeSpan>(),
                    It.IsAny<string[]>()
                ),
            Times.Never
        );
    }

    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task GetCategoriesQueryHandler_WhenValidQuery_ShouldReturnListOfCategoryDTO(
        IReadOnlyList<Category> categories
    )
    {
        // Arrange
        var request = new GetCategoriesQuery
        {
            Name = categories[0].Name,
            Keyword = categories[0].Name,
        };
        var redisKey = $"TestKey";
        _redisCache.Setup(mock => mock.GenerateKey(It.IsAny<object?[]>())).Returns(redisKey);
        _redisCache
            .Setup(mock => mock.GetCachedData<List<CategoryDTO>>(redisKey))
            .ReturnsAsync((List<CategoryDTO>)null!);
        _repository
            .Setup(mock => mock.ApplySpecification(It.IsAny<CategoriesSpecification>()))
            .Returns(categories.ToList().BuildMock())
            .Verifiable();
        _redisCache
            .Setup(mock =>
                mock.SetCachedData(
                    redisKey,
                    It.IsAny<List<CategoryDTO>>(),
                    It.IsAny<TimeSpan>(),
                    It.IsAny<string[]>()
                )
            )
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result
            .Should()
            .BeEquivalentTo(
                Result<List<CategoryDTO>>.Success([.. categories.Select(_mapper.Map<CategoryDTO>)])
            );
        _redisCache.Verify(mock => mock.GenerateKey(It.IsAny<object?[]>()), Times.Once());
        _redisCache.Verify(mock => mock.GetCachedData<List<CategoryDTO>>(redisKey), Times.Once);
        _repository.Verify(
            mock => mock.ApplySpecification(It.IsAny<CategoriesSpecification>()),
            Times.Once
        );
        _redisCache.Verify(
            mock =>
                mock.SetCachedData(
                    redisKey,
                    It.IsAny<List<CategoryDTO>>(),
                    It.IsAny<TimeSpan>(),
                    It.IsAny<string[]>()
                ),
            Times.Once
        );
    }
}
