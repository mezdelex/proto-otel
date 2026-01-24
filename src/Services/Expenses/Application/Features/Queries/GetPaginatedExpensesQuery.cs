namespace Application.Features.Queries;

public sealed record GetPaginatedExpensesQuery
    : BaseRequest,
        IRequest<Result<PaginatedList<ExtraExpenseDTO>>>
{
    public string? Name { get; set; }
    public string? Keyword { get; set; }
    public DateTime? MinDate { get; set; }
    public DateTime? MaxDate { get; set; }
    public string? CategoryId { get; set; }
    public string? ApplicationUserId { get; set; }
    public string? Email { get; set; }

    public sealed class GetPaginatedExpensesQueryHandler(
        IRedisCache redisCache,
        IExpensesRepository repository,
        IMapper mapper
    ) : IRequestHandler<GetPaginatedExpensesQuery, Result<PaginatedList<ExtraExpenseDTO>>>
    {
        private readonly IRedisCache _redisCache = redisCache;
        private readonly IExpensesRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<PaginatedList<ExtraExpenseDTO>>> Handle(
            GetPaginatedExpensesQuery request,
            CancellationToken cancellationToken
        )
        {
            var redisKey =
                $"{nameof(Expense)}#{request.Name}#{request.Keyword}#{request.MinDate}#{request.MaxDate}#{request.CategoryId}#{request.ApplicationUserId}#{request.Email}#{request.Page}#{request.PageSize}";
            var cachedPaginatedExpenses = await _redisCache.GetCachedData<
                PaginatedList<ExtraExpenseDTO>
            >(redisKey);
            if (cachedPaginatedExpenses != null)
            {
                return Result<PaginatedList<ExtraExpenseDTO>>.Success(cachedPaginatedExpenses);
            }

            var paginatedExpenses = await _repository
                .ApplySpecification(
                    new ExpensesSpecification(
                        name: request.Name,
                        keyword: request.Keyword,
                        minDate: request.MinDate,
                        maxDate: request.MaxDate,
                        categoryId: request.CategoryId,
                        applicationUserId: request.ApplicationUserId,
                        email: request.Email,
                        includes: _ => _.Include(x => x.ApplicationUser).Include(x => x.Category)
                    )
                )
                .AsNoTracking()
                .ProjectTo<ExtraExpenseDTO>(_mapper.ConfigurationProvider)
                .ToPaginatedListAsync(request.Page, request.PageSize, cancellationToken);

            await _redisCache.SetCachedData(
                redisKey,
                paginatedExpenses,
                DateTimeOffset.Now.AddMinutes(5)
            );

            return Result<PaginatedList<ExtraExpenseDTO>>.Success(paginatedExpenses);
        }
    }

    public class GetPaginatedExpensesQueryValidator : AbstractValidator<GetPaginatedExpensesQuery>
    {
        public GetPaginatedExpensesQueryValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(ExpenseConstraints.NameMaxLength)
                .WithMessage(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(Name),
                        ExpenseConstraints.NameMaxLength
                    )
                )
                .When(x => !string.IsNullOrWhiteSpace(x.Name));

            RuleFor(x => x.Keyword)
                .MaximumLength(ExpenseConstraints.DescriptionMaxLength)
                .WithMessage(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(Keyword),
                        ExpenseConstraints.DescriptionMaxLength
                    )
                )
                .When(x => !string.IsNullOrWhiteSpace(x.Keyword));

            RuleFor(x => x.CategoryId)
                .MaximumLength(CategoryConstraints.IdMaxLength)
                .WithMessage(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(CategoryId),
                        CategoryConstraints.IdMaxLength
                    )
                )
                .When(x => !string.IsNullOrWhiteSpace(x.CategoryId));

            RuleFor(x => x.ApplicationUserId)
                .MaximumLength(ApplicationUserConstraints.IdMaxLength)
                .WithMessage(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(ApplicationUserId),
                        ApplicationUserConstraints.IdMaxLength
                    )
                )
                .When(x => !string.IsNullOrWhiteSpace(x.ApplicationUserId));
        }
    }
}
