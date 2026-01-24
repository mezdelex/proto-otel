namespace UnitTests.Features.Queries;

public sealed class GetPaginatedExpensesQueryHandlerTests
{
    private readonly CancellationToken _cancellationToken;
    private readonly Mock<IRedisCache> _redisCache;
    private readonly Mock<IExpensesRepository> _repository;
    private readonly IMapper _mapper;
    private readonly ILoggerFactory _loggerFactory;
    private readonly GetPaginatedExpensesQueryHandler _handler;

    public GetPaginatedExpensesQueryHandlerTests()
    {
        _cancellationToken = new();
        _redisCache = new();
        _repository = new();
        _loggerFactory = new LoggerFactory();
        _mapper = new MapperConfiguration(
            c =>
            {
                c.AddProfile<ApplicationUsersProfile>();
                c.AddProfile<CategoriesProfile>();
                c.AddProfile<ExpensesProfile>();
            },
            _loggerFactory
        ).CreateMapper();

        _handler = new GetPaginatedExpensesQueryHandler(
            _redisCache.Object,
            _repository.Object,
            _mapper
        );
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpensesWithUsers), MemberType = typeof(ExpensesMock))]
    public async Task GetPaginatedExpensesQueryHandler_WhenExceptionIsThrown_ShouldPropagateException(
        IReadOnlyList<Expense> expenses,
        IReadOnlyList<ApplicationUser> _
    )
    {
        // Arrange
        var request = new GetPaginatedExpensesQuery
        {
            Name = expenses[0].Name,
            Keyword = expenses[0].Name,
            CategoryId = expenses[0].CategoryId,
            ApplicationUserId = expenses[0].ApplicationUserId,
            Page = 0,
            PageSize = expenses.Count,
        };
        var redisKey =
            $"{nameof(Expense)}#{request.Name}#{request.Keyword}#{request.MinDate}#{request.MaxDate}#{request.CategoryId}#{request.ApplicationUserId}#{request.Email}#{request.Page}#{request.PageSize}";
        _redisCache
            .Setup(mock => mock.GetCachedData<PaginatedList<ExtraExpenseDTO>>(redisKey))
            .ReturnsAsync((PaginatedList<ExtraExpenseDTO>)null!);
        _repository
            .Setup(mock => mock.ApplySpecification(It.IsAny<ExpensesSpecification>()))
            .Throws(new Exception())
            .Verifiable();
        _redisCache
            .Setup(mock =>
                mock.SetCachedData(
                    redisKey,
                    It.IsAny<PaginatedList<ExtraExpenseDTO>>(),
                    It.IsAny<DateTimeOffset>()
                )
            )
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act & Assert
        await _handler
            .Invoking(x => x.Handle(request, _cancellationToken))
            .Should()
            .ThrowAsync<Exception>();
        _redisCache.Verify(
            mock => mock.GetCachedData<PaginatedList<ExtraExpenseDTO>>(redisKey),
            Times.Once
        );
        _repository.Verify(
            mock => mock.ApplySpecification(It.IsAny<ExpensesSpecification>()),
            Times.Once
        );
        _redisCache.Verify(
            mock =>
                mock.SetCachedData(
                    redisKey,
                    It.IsAny<PaginatedList<ExtraExpenseDTO>>(),
                    It.IsAny<DateTimeOffset>()
                ),
            Times.Never
        );
    }

    [Theory]
    [MemberData(nameof(ExpensesMock.GetExpensesWithUsers), MemberType = typeof(ExpensesMock))]
    public async Task GetPaginatedExpensesQueryHandler_WhenValidQuery_ShouldReturnPaginatedListOfExtraExpenseDTO(
        IReadOnlyList<Expense> expenses,
        IReadOnlyList<ApplicationUser> _
    )
    {
        // Arrange
        var request = new GetPaginatedExpensesQuery
        {
            Name = expenses[0].Name,
            Keyword = expenses[0].Name,
            CategoryId = expenses[0].CategoryId,
            ApplicationUserId = expenses[0].ApplicationUserId,
            Page = 0,
            PageSize = expenses.Count,
        };
        var redisKey =
            $"{nameof(Expense)}#{request.Name}#{request.Keyword}#{request.MinDate}#{request.MaxDate}#{request.CategoryId}#{request.ApplicationUserId}#{request.Email}#{request.Page}#{request.PageSize}";
        _redisCache
            .Setup(mock => mock.GetCachedData<PaginatedList<ExtraExpenseDTO>>(redisKey))
            .ReturnsAsync((PaginatedList<ExtraExpenseDTO>)null!);
        _repository
            .Setup(mock => mock.ApplySpecification(It.IsAny<ExpensesSpecification>()))
            .Returns(expenses.ToList().BuildMock())
            .Verifiable();
        _redisCache
            .Setup(mock =>
                mock.SetCachedData(
                    redisKey,
                    It.IsAny<PaginatedList<ExtraExpenseDTO>>(),
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
                Result<PaginatedList<ExtraExpenseDTO>>.Success(
                    new PaginatedList<ExtraExpenseDTO>(
                        [.. expenses.Select(_mapper.Map<ExtraExpenseDTO>)],
                        0,
                        expenses.Count,
                        expenses.Count,
                        false,
                        true
                    )
                )
            );
        _repository.Verify(
            mock => mock.ApplySpecification(It.IsAny<ExpensesSpecification>()),
            Times.Once
        );
        _redisCache.Verify(
            mock => mock.GetCachedData<PaginatedList<ExtraExpenseDTO>>(redisKey),
            Times.Once
        );
        _redisCache.Verify(
            mock =>
                mock.SetCachedData(
                    redisKey,
                    It.IsAny<PaginatedList<ExtraExpenseDTO>>(),
                    It.IsAny<DateTimeOffset>()
                ),
            Times.Once
        );
    }
}
