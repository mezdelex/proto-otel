namespace UnitTests.Features.Commands;

public sealed class PatchExpenseCommandHandlerTests
{
    private readonly CancellationToken _cancellationToken;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IMapper _mapper;
    private readonly Mock<IExpensesRepository> _repository;
    private readonly Mock<IUnitOfWork> _uow;
    private readonly Mock<IRedisCache> _redisCache;
    private readonly Mock<IEventBus> _eventBus;
    private readonly PatchExpenseCommandHandler _handler;

    public PatchExpenseCommandHandlerTests()
    {
        _cancellationToken = new();
        _loggerFactory = new LoggerFactory();
        _mapper = new MapperConfiguration(
            c => c.AddProfile<ExpensesProfile>(),
            _loggerFactory
        ).CreateMapper();
        _repository = new();
        _uow = new();
        _redisCache = new();
        _eventBus = new();

        _handler = new PatchExpenseCommandHandler(
            _mapper,
            _repository.Object,
            _uow.Object,
            _redisCache.Object,
            _eventBus.Object
        );
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task PatchExpenseCommandHandler_WhenExpenseToPatchNotFound_ShouldReturnNotFoundResultErrorAsync(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new PatchExpenseCommand(
            expenses[0].Id,
            expenses[0].Name,
            expenses[0].Description,
            expenses[0].Value,
            expenses[0].Date,
            expenses[0].CategoryId,
            expenses[0].ApplicationUserId
        );
        _repository
            .Setup(mock => mock.PatchAsync(It.IsAny<Expense>(), _cancellationToken))
            .ReturnsAsync(
                Result<Empty>.Error([
                    new Error(
                        Errors.NotFoundError,
                        Errors.NotFoundErrorDetail(nameof(Expense)),
                        ErrorTypes.NotFound
                    ),
                ])
            )
            .Verifiable();
        _uow.Setup(mock => mock.SaveChangesAsync(_cancellationToken)).Verifiable();
        _redisCache.Setup(mock => mock.RemoveKeysByTags(It.IsAny<string>())).Verifiable();
        _eventBus
            .Setup(mock => mock.PublishAsync(It.IsAny<PatchedExpenseEvent>(), _cancellationToken))
            .Verifiable();

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result
            .Should()
            .BeEquivalentTo(
                Result<Empty>.Error([
                    new Error(
                        Errors.NotFoundError,
                        Errors.NotFoundErrorDetail(nameof(Expense)),
                        ErrorTypes.NotFound
                    ),
                ])
            );
        _repository.Verify(
            mock => mock.PatchAsync(It.IsAny<Expense>(), _cancellationToken),
            Times.Once
        );
        _uow.Verify(mock => mock.SaveChangesAsync(_cancellationToken), Times.Never);
        _redisCache.Verify(mock => mock.RemoveKeysByTags(It.IsAny<string>()), Times.Never);
        _eventBus.Verify(
            mock => mock.PublishAsync(It.IsAny<PatchedExpenseEvent>(), _cancellationToken),
            Times.Never
        );
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task PatchExpenseCommandHandler_WhenExceptionIsThrown_ShouldPropagateException(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new PatchExpenseCommand(
            expenses[0].Id,
            expenses[0].Name,
            expenses[0].Description,
            expenses[0].Value,
            expenses[0].Date,
            expenses[0].CategoryId,
            expenses[0].ApplicationUserId
        );
        _repository
            .Setup(mock => mock.PatchAsync(It.IsAny<Expense>(), _cancellationToken))
            .ThrowsAsync(new Exception())
            .Verifiable();
        _uow.Setup(mock => mock.SaveChangesAsync(_cancellationToken)).Verifiable();
        _redisCache.Setup(mock => mock.RemoveKeysByTags(It.IsAny<string>())).Verifiable();
        _eventBus
            .Setup(mock => mock.PublishAsync(It.IsAny<PatchedExpenseEvent>(), _cancellationToken))
            .Verifiable();

        // Act & Assert
        await _handler
            .Invoking(x => x.Handle(request, _cancellationToken))
            .Should()
            .ThrowAsync<Exception>();
        _repository.Verify(
            mock => mock.PatchAsync(It.IsAny<Expense>(), _cancellationToken),
            Times.Once
        );
        _uow.Verify(mock => mock.SaveChangesAsync(_cancellationToken), Times.Never);
        _redisCache.Verify(mock => mock.RemoveKeysByTags(It.IsAny<string>()), Times.Never);
        _eventBus.Verify(
            mock => mock.PublishAsync(It.IsAny<PatchedExpenseEvent>(), _cancellationToken),
            Times.Never
        );
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task PatchExpenseCommandHandler_WhenValidCommand_ShouldPatchExpenseAndPublishEventAsync(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new PatchExpenseCommand(
            expenses[0].Id,
            expenses[0].Name,
            expenses[0].Description,
            expenses[0].Value,
            expenses[0].Date,
            expenses[0].CategoryId,
            expenses[0].ApplicationUserId
        );
        _repository
            .Setup(mock => mock.PatchAsync(It.IsAny<Expense>(), _cancellationToken))
            .ReturnsAsync(Result<Empty>.Success())
            .Verifiable();
        _uow.Setup(mock => mock.SaveChangesAsync(_cancellationToken)).Verifiable();
        _redisCache.Setup(mock => mock.RemoveKeysByTags(It.IsAny<string>())).Verifiable();
        _eventBus
            .Setup(mock => mock.PublishAsync(It.IsAny<PatchedExpenseEvent>(), _cancellationToken))
            .Verifiable();

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(Result<Empty>.Success());
        _repository.Verify(
            mock => mock.PatchAsync(It.IsAny<Expense>(), _cancellationToken),
            Times.Once
        );
        _uow.Verify(mock => mock.SaveChangesAsync(_cancellationToken), Times.Once);
        _redisCache.Verify(mock => mock.RemoveKeysByTags(It.IsAny<string>()), Times.Once);
        _eventBus.Verify(
            mock => mock.PublishAsync(It.IsAny<PatchedExpenseEvent>(), _cancellationToken),
            Times.Once
        );
    }
}
