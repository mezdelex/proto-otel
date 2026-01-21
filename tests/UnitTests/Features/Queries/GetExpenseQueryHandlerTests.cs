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
    public async Task Handle_ValidIdGetExpenseQuery_ShouldReturnRequestedExpenseAsExpenseDTOAsync(
        IEnumerable<Expense> expenses,
        IEnumerable<ApplicationUser> _
    )
    {
        // Arrange
        var request = new GetExpenseQuery(expenses.First().Id);
        _repository
            .Setup(mock =>
                mock.GetBySpecAsync(It.IsAny<ExpensesSpecification>(), _cancellationToken)
            )
            .ReturnsAsync(expenses.First())
            .Verifiable();

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(_mapper.Map<ExpenseDTO>(expenses.First()));
        _repository.Verify(
            mock => mock.GetBySpecAsync(It.IsAny<ExpensesSpecification>(), _cancellationToken),
            Times.Once
        );
    }
}
