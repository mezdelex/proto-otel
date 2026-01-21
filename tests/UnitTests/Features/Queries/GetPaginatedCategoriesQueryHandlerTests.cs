namespace UnitTests.Features.Queries;

public sealed class GetPaginatedCategoriesQueryHandlerTests
{
    private readonly CancellationToken _cancellationToken;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IMapper _mapper;
    private readonly Mock<ICategoriesRepository> _repository;
    private readonly Mock<IRedisCache> _redisCache;
    private readonly GetPaginatedCategoriesQueryHandler _handler;

    public GetPaginatedCategoriesQueryHandlerTests()
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

        _handler = new GetPaginatedCategoriesQueryHandler(
            _repository.Object,
            _mapper,
            _redisCache.Object
        );
    }

    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task GetPaginatedCategoriesQueryHandler_ShouldReturnPaginatedCategoryDTO(
        IEnumerable<Category> categories
    )
    {
        // Arrange
        var request = new GetPaginatedCategoriesQuery
        {
            Name = categories.First().Name,
            ContainedWord = categories.First().Name,
            Page = 0,
            PageSize = categories.Count(),
        };
        var redisKey =
            $"{nameof(Category)}#{request.Name}#{request.ContainedWord}#{request.Page}#{request.PageSize}";
        _repository
            .Setup(mock => mock.ApplySpecification(It.IsAny<CategoriesSpecification>()))
            .Returns(categories.ToList().BuildMock())
            .Verifiable();
        _redisCache
            .Setup(mock => mock.GetCachedData<PaginatedList<CategoryDTO>>(redisKey))
            .ReturnsAsync((PaginatedList<CategoryDTO>)null!);
        _redisCache
            .Setup(mock =>
                mock.SetCachedData(
                    redisKey,
                    It.IsAny<PaginatedList<CategoryDTO>>(),
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
                new PaginatedList<CategoryDTO>(
                    [.. categories.Select(_mapper.Map<CategoryDTO>)],
                    0,
                    categories.Count(),
                    categories.Count(),
                    false,
                    true
                )
            );
        _repository.Verify(
            mock => mock.ApplySpecification(It.IsAny<CategoriesSpecification>()),
            Times.Once
        );
        _redisCache.Verify(
            mock => mock.GetCachedData<PaginatedList<CategoryDTO>>(redisKey),
            Times.Once
        );
        _redisCache.Verify(
            mock =>
                mock.SetCachedData(
                    redisKey,
                    It.IsAny<PaginatedList<CategoryDTO>>(),
                    It.IsAny<DateTimeOffset>()
                ),
            Times.Once
        );
    }
}
