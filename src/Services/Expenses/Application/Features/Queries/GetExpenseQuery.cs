namespace Application.Features.Queries;

public sealed record GetExpenseQuery(string Id) : IRequest<Result<ExpenseDTO>>
{
    public sealed class GetExpenseQueryHandler(IExpensesRepository repository, IMapper mapper)
        : IRequestHandler<GetExpenseQuery, Result<ExpenseDTO>>
    {
        private readonly IExpensesRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<ExpenseDTO>> Handle(
            GetExpenseQuery request,
            CancellationToken cancellationToken
        )
        {
            var expense = await _repository.GetBySpecAsync(
                new ExpensesSpecification(id: request.Id),
                cancellationToken
            );
            if (expense is null)
            {
                return Result<ExpenseDTO>.Failure(
                    new Error(
                        Errors.NotFoundError,
                        Errors.NotFoundErrorDetail(nameof(Expense)),
                        ErrorTypes.NotFound
                    )
                );
            }

            return Result<ExpenseDTO>.Success(_mapper.Map<ExpenseDTO>(expense));
        }
    }

    public class GetExpenseQueryValidator : AbstractValidator<GetExpenseQuery>
    {
        public GetExpenseQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage(GenericValidationMessages.ShouldNotBeEmpty(nameof(Id)))
                .MaximumLength(ExpenseConstraints.IdMaxLength)
                .WithMessage(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(Id),
                        ExpenseConstraints.IdMaxLength
                    )
                );
        }
    }
}
