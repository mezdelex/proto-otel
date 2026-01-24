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
    public async Task Handle_ValidIdGetCategoryQuery_ShouldReturnRequestedCategoryAsCategoryDTOAsync(
        IEnumerable<Category> categories
    )
    {
        // Arrange
        var request = new GetCategoryQuery(categories.First().Id);
        _repository
            .Setup(mock =>
                mock.GetBySpecAsync(It.IsAny<CategoriesSpecification>(), _cancellationToken)
            )
            .ReturnsAsync(categories.First())
            .Verifiable();

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result
            .Should()
            .BeEquivalentTo(
                Result<CategoryDTO>.Success(_mapper.Map<CategoryDTO>(categories.First()))
            );
        _repository.Verify(
            mock => mock.GetBySpecAsync(It.IsAny<CategoriesSpecification>(), _cancellationToken),
            Times.Once
        );
    }
}
