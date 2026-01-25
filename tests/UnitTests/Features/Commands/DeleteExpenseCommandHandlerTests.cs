namespace UnitTests.Features.Commands;

public sealed class DeleteExpenseCommandHandlerTests
{
    private readonly CancellationToken _cancellationToken;
    private readonly Mock<IExpensesRepository> _repository;
    private readonly Mock<IUnitOfWork> _uow;
    private readonly Mock<IRedisCache> _redisCache;
    private readonly DeleteExpenseCommandHandler _handler;

    public DeleteExpenseCommandHandlerTests()
    {
        _cancellationToken = new();
        _repository = new();
        _uow = new();
        _redisCache = new();

        _handler = new DeleteExpenseCommandHandler(
            _repository.Object,
            _uow.Object,
            _redisCache.Object
        );
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task DeleteExpenseCommandHandler_WhenExpenseToDeleteNotFound_ShouldReturnNotFoundResultErrorAsync(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new DeleteExpenseCommand(expenses[0].Id);
        _repository
            .Setup(mock => mock.DeleteAsync(It.IsAny<string>(), _cancellationToken))
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
        _redisCache.Setup(mock => mock.RemoveKeysByPattern(It.IsAny<string>())).Verifiable();

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
            mock => mock.DeleteAsync(It.IsAny<string>(), _cancellationToken),
            Times.Once
        );
        _uow.Verify(mock => mock.SaveChangesAsync(_cancellationToken), Times.Never);
        _redisCache.Verify(mock => mock.RemoveKeysByPattern(It.IsAny<string>()), Times.Never);
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task DeleteExpenseCommandHandler_WhenExceptionIsThrown_ShouldPropagateException(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new DeleteExpenseCommand(expenses[0].Id);
        _repository
            .Setup(mock => mock.DeleteAsync(It.IsAny<string>(), _cancellationToken))
            .ThrowsAsync(new Exception())
            .Verifiable();
        _uow.Setup(mock => mock.SaveChangesAsync(_cancellationToken)).Verifiable();
        _redisCache.Setup(mock => mock.RemoveKeysByPattern(It.IsAny<string>())).Verifiable();

        // Act & Assert
        await _handler
            .Invoking(x => x.Handle(request, _cancellationToken))
            .Should()
            .ThrowAsync<Exception>();
        _repository.Verify(
            mock => mock.DeleteAsync(It.IsAny<string>(), _cancellationToken),
            Times.Once
        );
        _uow.Verify(mock => mock.SaveChangesAsync(_cancellationToken), Times.Never);
        _redisCache.Verify(mock => mock.RemoveKeysByPattern(It.IsAny<string>()), Times.Never);
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task DeleteExpenseCommandHandler_WhenValidCommand_ShouldDeleteExpense(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new DeleteExpenseCommand(expenses[0].Id);
        _repository
            .Setup(mock => mock.DeleteAsync(It.IsAny<string>(), _cancellationToken))
            .ReturnsAsync(Result<Empty>.Success())
            .Verifiable();
        _uow.Setup(mock => mock.SaveChangesAsync(_cancellationToken)).Verifiable();
        _redisCache.Setup(mock => mock.RemoveKeysByPattern(It.IsAny<string>())).Verifiable();

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(Result<Empty>.Success());
        _repository.Verify(
            mock => mock.DeleteAsync(It.IsAny<string>(), _cancellationToken),
            Times.Once
        );
        _uow.Verify(mock => mock.SaveChangesAsync(_cancellationToken), Times.Once);
        _redisCache.Verify(mock => mock.RemoveKeysByPattern(It.IsAny<string>()), Times.Once);
    }
}
