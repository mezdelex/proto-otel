namespace UnitTests.Features.Queries;

public sealed class GetCategoriesQueryHandlerTests
{
    private readonly CancellationToken _cancellationToken;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IMapper _mapper;
    private readonly Mock<ICategoriesRepository> _repository;
    private readonly Mock<IRedisCache> _redisCache;
    private readonly GetCategoriesQueryHandler _handler;

    public GetCategoriesQueryHandlerTests()
    {
        _cancellationToken = new();
        _loggerFactory = new LoggerFactory();
        _mapper = new MapperConfiguration(
            c =>
            {
                c.AddProfile<CategoriesProfile>();
                c.AddProfile<ExpensesProfile>();
            },
            _loggerFactory
        ).CreateMapper();
        _repository = new();
        _redisCache = new();

        _handler = new GetCategoriesQueryHandler(_repository.Object, _mapper, _redisCache.Object);
    }

    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task GetCategoriesQueryHandler_ShouldReturnListOfCategoryDTO(
        IEnumerable<Category> categories
    )
    {
        // Arrange
        var request = new GetCategoriesQuery
        {
            Name = categories.First().Name,
            ContainedWord = categories.First().Name,
        };
        var redisKey = $"{nameof(Category)}#{request.Name}#{request.ContainedWord}";
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
        result.Should().BeEquivalentTo([.. categories.Select(_mapper.Map<CategoryDTO>)]);
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
