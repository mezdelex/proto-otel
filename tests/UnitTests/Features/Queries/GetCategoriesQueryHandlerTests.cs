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
    public async Task GetCategoriesQueryHandler_ShouldReturnListOfCategoryDTO(
        IReadOnlyList<Category> categories
    )
    {
        // Arrange
        var request = new GetCategoriesQuery
        {
            Name = categories[0].Name,
            Keyword = categories[0].Name,
        };
        var redisKey = $"{nameof(Category)}#{request.Name}#{request.Keyword}";
        _repository
            .Setup(mock => mock.ApplySpecification(It.IsAny<CategoriesSpecification>()))
            .Returns(categories.ToList().BuildMock())
            .Verifiable();
        _redisCache
            .Setup(mock => mock.GetCachedData<List<CategoryDTO>>(redisKey))
            .ReturnsAsync((List<CategoryDTO>)null!);
        _redisCache
            .Setup(mock =>
                mock.SetCachedData(
                    redisKey,
                    It.IsAny<List<CategoryDTO>>(),
                    It.IsAny<DateTimeOffset>()
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
        _repository.Verify(
            mock => mock.ApplySpecification(It.IsAny<CategoriesSpecification>()),
            Times.Once
        );
        _redisCache.Verify(mock => mock.GetCachedData<List<CategoryDTO>>(redisKey), Times.Once);
        _redisCache.Verify(
            mock =>
                mock.SetCachedData(
                    redisKey,
                    It.IsAny<List<CategoryDTO>>(),
                    It.IsAny<DateTimeOffset>()
                ),
            Times.Once
        );
    }
}
