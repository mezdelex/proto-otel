namespace UnitTests.Features.Queries;

public sealed class GetExpenseQueryHandlerTests
{
    private readonly CancellationToken _cancellationToken;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IMapper _mapper;
    private readonly Mock<IExpensesRepository> _repository;
    private readonly GetExpenseQueryHandler _handler;

    public GetExpenseQueryHandlerTests()
    {
        _cancellationToken = new();
        _loggerFactory = new LoggerFactory();
        _mapper = new MapperConfiguration(
            c => c.AddProfile<ExpensesProfile>(),
            _loggerFactory
        ).CreateMapper();
        _repository = new();
        _handler = new GetExpenseQueryHandler(_repository.Object, _mapper);
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpensesWithUsers), MemberType = typeof(ExpensesMock))]
    public async Task GetExpenseQuery_WhenExceptionIsThrown_ShouldPropagateException(
        IReadOnlyList<Expense> expenses,
        IReadOnlyList<ApplicationUser> _
    )
    {
        // Arrange
        var request = new GetExpenseQuery(expenses[0].Id);
        _repository
            .Setup(mock =>
                mock.GetBySpecAsync(It.IsAny<ExpensesSpecification>(), _cancellationToken)
            )
            .ThrowsAsync(new Exception())
            .Verifiable();

        // Act & Assert
        await _handler
            .Invoking(x => x.Handle(request, _cancellationToken))
            .Should()
            .ThrowAsync<Exception>();
        _repository.Verify(
            mock => mock.GetBySpecAsync(It.IsAny<ExpensesSpecification>(), _cancellationToken),
            Times.Once
        );
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpensesWithUsers), MemberType = typeof(ExpensesMock))]
    public async Task GetExpenseQuery_WhenExpenseNotFound_ShouldReturnNotFoundResultErrorAsync(
        IReadOnlyList<Expense> expenses,
        IReadOnlyList<ApplicationUser> _
    )
    {
        // Arrange
        var request = new GetExpenseQuery(expenses[0].Id);
        _repository
            .Setup(mock =>
                mock.GetBySpecAsync(It.IsAny<ExpensesSpecification>(), _cancellationToken)
            )
            .ReturnsAsync(null as Expense)
            .Verifiable();

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result
            .Should()
            .BeEquivalentTo(
                Result<ExpenseDTO>.Error([
                    new Error(
                        Errors.NotFoundError,
                        Errors.NotFoundErrorDetail(nameof(Expense)),
                        ErrorTypes.NotFound
                    ),
                ])
            );
        _repository.Verify(
            mock => mock.GetBySpecAsync(It.IsAny<ExpensesSpecification>(), _cancellationToken),
            Times.Once
        );
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpensesWithUsers), MemberType = typeof(ExpensesMock))]
    public async Task GetExpenseQuery_WhenValidQuery_ShouldReturnExpenseDTOAsync(
        IReadOnlyList<Expense> expenses,
        IReadOnlyList<ApplicationUser> _
    )
    {
        // Arrange
        var request = new GetExpenseQuery(expenses[0].Id);
        _repository
            .Setup(mock =>
                mock.GetBySpecAsync(It.IsAny<ExpensesSpecification>(), _cancellationToken)
            )
            .ReturnsAsync(expenses[0])
            .Verifiable();

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result
            .Should()
            .BeEquivalentTo(Result<ExpenseDTO>.Success(_mapper.Map<ExpenseDTO>(expenses[0])));
        _repository.Verify(
            mock => mock.GetBySpecAsync(It.IsAny<ExpensesSpecification>(), _cancellationToken),
            Times.Once
        );
    }
}
