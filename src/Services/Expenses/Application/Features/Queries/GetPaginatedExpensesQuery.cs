namespace Application.Features.Queries;

public sealed record GetPaginatedExpensesQuery : BaseRequest, IRequest<PaginatedList<ExpenseDTO>>
{
    public string? Name { get; set; }
    public string? ContainedWord { get; set; }
    public DateTime? MinDate { get; set; }
    public DateTime? MaxDate { get; set; }
    public string? CategoryId { get; set; }
    public string? ApplicationUserId { get; set; }
    public string? Email { get; set; }

    public sealed class GetPaginatedExpensesQueryHandler(
        IExpensesRepository repository,
        IMapper mapper,
        IRedisCache redisCache
    ) : IRequestHandler<GetPaginatedExpensesQuery, PaginatedList<ExpenseDTO>>
    {
        private readonly IExpensesRepository _repository = repository;
        private readonly IMapper _mapper = mapper;
        private readonly IRedisCache _redisCache = redisCache;

        public async Task<PaginatedList<ExpenseDTO>> Handle(
            GetPaginatedExpensesQuery request,
            CancellationToken cancellationToken
        )
        {
            var redisKey =
                $"{nameof(Expense)}#{request.Name}#{request.ContainedWord}#{request.MinDate}#{request.MaxDate}#{request.CategoryId}#{request.ApplicationUserId}#{request.Email}#{request.Page}#{request.PageSize}";
            var cachedPaginatedExpenses = await _redisCache.GetCachedData<
                PaginatedList<ExpenseDTO>
            >(redisKey);
            if (cachedPaginatedExpenses != null)
            {
                return cachedPaginatedExpenses;
            }

            var paginatedExpenses = await _repository
                .ApplySpecification(
                    new ExpensesSpecification(
                        name: request.Name,
                        containedWord: request.ContainedWord,
                        minDate: request.MinDate,
                        maxDate: request.MaxDate,
                        categoryId: request.CategoryId,
                        applicationUserId: request.ApplicationUserId,
                        email: request.Email
                    )
                )
                .AsNoTracking()
                .ProjectTo<ExpenseDTO>(_mapper.ConfigurationProvider)
                .ToPaginatedListAsync(request.Page, request.PageSize, cancellationToken);

            await _redisCache.SetCachedData(
                redisKey,
                paginatedExpenses,
                DateTimeOffset.Now.AddMinutes(5)
            );

            return paginatedExpenses;
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

            RuleFor(x => x.ContainedWord)
                .MaximumLength(ExpenseConstraints.DescriptionMaxLength)
                .WithMessage(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(ContainedWord),
                        ExpenseConstraints.DescriptionMaxLength
                    )
                )
                .When(x => !string.IsNullOrWhiteSpace(x.ContainedWord));

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
