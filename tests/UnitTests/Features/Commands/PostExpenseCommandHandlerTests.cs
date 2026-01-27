namespace UnitTests.Features.Commands;

public sealed class PostExpenseCommandHandlerTests
{
    private readonly CancellationToken _cancellationToken;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
    private readonly Mock<IApplicationUsersRepository> _applicationUserRepository;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IMapper _mapper;
    private readonly Mock<IExpensesRepository> _repository;
    private readonly Mock<IUnitOfWork> _uow;
    private readonly Mock<IRedisCache> _redisCache;
    private readonly Mock<IEventBus> _eventBus;
    private readonly PostExpenseCommandHandler _handler;

    public PostExpenseCommandHandlerTests()
    {
        _cancellationToken = new();
        _httpContextAccessor = new();
        _applicationUserRepository = new();
        _loggerFactory = new LoggerFactory();
        _mapper = new MapperConfiguration(
            c => c.AddProfile<ExpensesProfile>(),
            _loggerFactory
        ).CreateMapper();
        _repository = new();
        _uow = new();
        _redisCache = new();
        _eventBus = new();

        _handler = new PostExpenseCommandHandler(
            _httpContextAccessor.Object,
            _applicationUserRepository.Object,
            _mapper,
            _repository.Object,
            _uow.Object,
            _redisCache.Object,
            _eventBus.Object
        );
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task PostExpenseCommandHandler_WhenApplicationUserNotFound_ShouldReturnNotFoundResultErrorAsync(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new PostExpenseCommand(
            expenses[0].Name,
            expenses[0].Description,
            expenses[0].Value,
            expenses[0].CategoryId
        );
        _httpContextAccessor
            .Setup(mock => mock.HttpContext.User.Identity!.Name)
            .Returns(expenses[0].ApplicationUser.Email)
            .Verifiable();
        _applicationUserRepository
            .Setup(mock =>
                mock.GetBySpecAsync(It.IsAny<ApplicationUsersSpecification>(), _cancellationToken)
            )
            .ReturnsAsync(null as ApplicationUser)
            .Verifiable();
        _repository
            .Setup(mock => mock.PostAsync(It.IsAny<Expense>(), _cancellationToken))
            .Verifiable();
        _uow.Setup(mock => mock.SaveChangesAsync(_cancellationToken)).Verifiable();
        _redisCache.Setup(mock => mock.RemoveKeysByTags(It.IsAny<string>())).Verifiable();
        _eventBus
            .Setup(mock => mock.PublishAsync(It.IsAny<PostedExpenseEvent>(), _cancellationToken))
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
                        Errors.NotFoundErrorDetail(nameof(ApplicationUser)),
                        ErrorTypes.NotFound
                    ),
                ])
            );
        _httpContextAccessor.Verify(mock => mock.HttpContext.User.Identity!.Name, Times.Once());
        _applicationUserRepository.Verify(
            mock =>
                mock.GetBySpecAsync(It.IsAny<ApplicationUsersSpecification>(), _cancellationToken),
            Times.Once
        );
        _repository.Verify(
            mock => mock.PostAsync(It.IsAny<Expense>(), _cancellationToken),
            Times.Never
        );
        _uow.Verify(mock => mock.SaveChangesAsync(_cancellationToken), Times.Never);
        _redisCache.Verify(mock => mock.RemoveKeysByTags(It.IsAny<string>()), Times.Never);
        _eventBus.Verify(
            mock => mock.PublishAsync(It.IsAny<PostedExpenseEvent>(), _cancellationToken),
            Times.Never
        );
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task PostExpenseCommandHandler_WhenExceptionIsThrown_ShoulPropagateException(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new PostExpenseCommand(
            expenses[0].Name,
            expenses[0].Description,
            expenses[0].Value,
            expenses[0].CategoryId
        );
        _httpContextAccessor
            .Setup(mock => mock.HttpContext.User.Identity!.Name)
            .Returns(expenses[0].ApplicationUser.Email)
            .Verifiable();
        _applicationUserRepository
            .Setup(mock =>
                mock.GetBySpecAsync(It.IsAny<ApplicationUsersSpecification>(), _cancellationToken)
            )
            .ThrowsAsync(new Exception())
            .Verifiable();
        _repository
            .Setup(mock => mock.PostAsync(It.IsAny<Expense>(), _cancellationToken))
            .Verifiable();
        _uow.Setup(mock => mock.SaveChangesAsync(_cancellationToken)).Verifiable();
        _redisCache.Setup(mock => mock.RemoveKeysByTags(It.IsAny<string>())).Verifiable();
        _eventBus
            .Setup(mock => mock.PublishAsync(It.IsAny<PostedExpenseEvent>(), _cancellationToken))
            .Verifiable();

        // Act & Assert
        await _handler
            .Invoking(x => x.Handle(request, _cancellationToken))
            .Should()
            .ThrowAsync<Exception>();
        _httpContextAccessor.Verify(mock => mock.HttpContext.User.Identity!.Name, Times.Once());
        _applicationUserRepository.Verify(
            mock =>
                mock.GetBySpecAsync(It.IsAny<ApplicationUsersSpecification>(), _cancellationToken),
            Times.Once
        );
        _repository.Verify(
            mock => mock.PostAsync(It.IsAny<Expense>(), _cancellationToken),
            Times.Never
        );
        _uow.Verify(mock => mock.SaveChangesAsync(_cancellationToken), Times.Never);
        _redisCache.Verify(mock => mock.RemoveKeysByTags(It.IsAny<string>()), Times.Never);
        _eventBus.Verify(
            mock => mock.PublishAsync(It.IsAny<PostedExpenseEvent>(), _cancellationToken),
            Times.Never
        );
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpenses), MemberType = typeof(ExpensesMock))]
    public async Task PostExpenseCommandHandler_WhenValidCommand_ShouldPostExpenseAndPublishEventAsync(
        IReadOnlyList<Expense> expenses
    )
    {
        // Arrange
        var request = new PostExpenseCommand(
            expenses[0].Name,
            expenses[0].Description,
            expenses[0].Value,
            expenses[0].CategoryId
        );
        _httpContextAccessor
            .Setup(mock => mock.HttpContext.User.Identity!.Name)
            .Returns(expenses[0].ApplicationUser.Email)
            .Verifiable();
        _applicationUserRepository
            .Setup(mock =>
                mock.GetBySpecAsync(It.IsAny<ApplicationUsersSpecification>(), _cancellationToken)
            )
            .ReturnsAsync(expenses[0].ApplicationUser)
            .Verifiable();
        _repository
            .Setup(mock => mock.PostAsync(It.IsAny<Expense>(), _cancellationToken))
            .Verifiable();
        _uow.Setup(mock => mock.SaveChangesAsync(_cancellationToken)).Verifiable();
        _redisCache.Setup(mock => mock.RemoveKeysByTags(It.IsAny<string>())).Verifiable();
        _eventBus
            .Setup(mock => mock.PublishAsync(It.IsAny<PostedExpenseEvent>(), _cancellationToken))
            .Verifiable();

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(Result<Empty>.Success());
        _httpContextAccessor.Verify(mock => mock.HttpContext.User.Identity!.Name, Times.Once());
        _applicationUserRepository.Verify(
            mock =>
                mock.GetBySpecAsync(It.IsAny<ApplicationUsersSpecification>(), _cancellationToken),
            Times.Once
        );
        _repository.Verify(
            mock => mock.PostAsync(It.IsAny<Expense>(), _cancellationToken),
            Times.Once
        );
        _uow.Verify(mock => mock.SaveChangesAsync(_cancellationToken), Times.Once);
        _redisCache.Verify(mock => mock.RemoveKeysByTags(It.IsAny<string>()), Times.Once());
        _eventBus.Verify(
            mock => mock.PublishAsync(It.IsAny<PostedExpenseEvent>(), _cancellationToken),
            Times.Once
        );
    }
}
