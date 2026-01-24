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
    [MemberData(nameof(ExpensesMock.GetExpensesWithUsers), MemberType = typeof(ExpensesMock))]
    public async Task PostExpenseCommandHandler_ShouldPostExpenseAndPublishEventAsync(
        IEnumerable<Expense> expenses,
        IEnumerable<ApplicationUser> users
    )
    {
        // Arrange
        var request = new PostExpenseCommand(
            expenses.First().Name,
            expenses.First().Description,
            expenses.First().Value,
            expenses.First().CategoryId
        );
        _httpContextAccessor
            .Setup(mock => mock.HttpContext.User.Identity!.Name)
            .Returns(users.First().Email)
            .Verifiable();
        _applicationUserRepository
            .Setup(mock =>
                mock.GetBySpecAsync(It.IsAny<ApplicationUsersSpecification>(), _cancellationToken)
            )
            .ReturnsAsync(users.First());
        _repository
            .Setup(mock => mock.PostAsync(It.IsAny<Expense>(), _cancellationToken))
            .Verifiable();
        _uow.Setup(mock => mock.SaveChangesAsync(_cancellationToken)).Verifiable();
        _redisCache.Setup(mock => mock.RemoveKeysByPattern(It.IsAny<string>())).Verifiable();
        _eventBus
            .Setup(mock => mock.PublishAsync(It.IsAny<PostedExpenseEvent>(), _cancellationToken))
            .Verifiable();

        // Act
        await _handler.Handle(request, _cancellationToken);

        // Assert
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
        _redisCache.Verify(mock => mock.RemoveKeysByPattern(It.IsAny<string>()), Times.Once());
        _eventBus.Verify(
            mock => mock.PublishAsync(It.IsAny<PostedExpenseEvent>(), _cancellationToken),
            Times.Once
        );
    }
}
