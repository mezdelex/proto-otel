namespace UnitTests.Features.Queries;

public sealed class GetCategoryQueryHandlerTests
{
    private readonly CancellationToken _cancellationToken;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IMapper _mapper;
    private readonly Mock<ICategoriesRepository> _repository;
    private readonly GetCategoryQueryHandler _handler;

    public GetCategoryQueryHandlerTests()
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

        _handler = new GetCategoryQueryHandler(_repository.Object, _mapper);
    }

    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task GetCategoryQuery_WhenCategoryNotFound_ShouldReturnNotFoundResultErrorAsync(
        IReadOnlyList<Category> categories
    )
    {
        // Arrange
        var request = new GetCategoryQuery(categories[0].Id);
        _repository
            .Setup(mock =>
                mock.GetBySpecAsync(It.IsAny<CategoriesSpecification>(), _cancellationToken)
            )
            .ReturnsAsync(null as Category)
            .Verifiable();

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result
            .Should()
            .BeEquivalentTo(
                Result<CategoryDTO>.Failure(
                    new Error(
                        Errors.NotFoundError,
                        Errors.NotFoundErrorDetail(nameof(Category)),
                        ErrorTypes.NotFound
                    )
                )
            );
        _repository.Verify(
            mock => mock.GetBySpecAsync(It.IsAny<CategoriesSpecification>(), _cancellationToken),
            Times.Once
        );
    }

    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task GetCategoryQuery_WhenExceptionIsThrown_ShouldPropagateException(
        IReadOnlyList<Category> categories
    )
    {
        // Arrange
        var request = new GetCategoryQuery(categories[0].Id);
        _repository
            .Setup(mock =>
                mock.GetBySpecAsync(It.IsAny<CategoriesSpecification>(), _cancellationToken)
            )
            .ThrowsAsync(new Exception())
            .Verifiable();

        // Act & Assert
        await _handler
            .Invoking(x => x.Handle(request, _cancellationToken))
            .Should()
            .ThrowAsync<Exception>();
        _repository.Verify(
            mock => mock.GetBySpecAsync(It.IsAny<CategoriesSpecification>(), _cancellationToken),
            Times.Once
        );
    }

    [Theory]
    [MemberData(nameof(CategoriesMock.GetCategories), MemberType = typeof(CategoriesMock))]
    public async Task GetCategoryQuery_WhenValidQuery_ShouldReturnCategoryDTOAsync(
        IReadOnlyList<Category> categories
    )
    {
        // Arrange
        var request = new GetCategoryQuery(categories[0].Id);
        _repository
            .Setup(mock =>
                mock.GetBySpecAsync(It.IsAny<CategoriesSpecification>(), _cancellationToken)
            )
            .ReturnsAsync(categories[0])
            .Verifiable();

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result
            .Should()
            .BeEquivalentTo(Result<CategoryDTO>.Success(_mapper.Map<CategoryDTO>(categories[0])));
        _repository.Verify(
            mock => mock.GetBySpecAsync(It.IsAny<CategoriesSpecification>(), _cancellationToken),
            Times.Once
        );
    }
}
