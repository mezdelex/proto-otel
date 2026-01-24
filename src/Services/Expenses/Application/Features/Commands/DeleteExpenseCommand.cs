namespace Application.Features.Commands;

public record DeleteExpenseCommand(string Id) : IRequest<Result<Empty>>
{
    public sealed class DeleteExpenseCommandHandler(
        IExpensesRepository repository,
        IUnitOfWork uow,
        IRedisCache redisCache
    ) : IRequestHandler<DeleteExpenseCommand, Result<Empty>>
    {
        private readonly IExpensesRepository _repository = repository;
        private readonly IUnitOfWork _uow = uow;
        private readonly IRedisCache _redisCache = redisCache;

        public async Task<Result<Empty>> Handle(
            DeleteExpenseCommand request,
            CancellationToken cancellationToken
        )
        {
            var result = await _repository.DeleteAsync(request.Id, cancellationToken);
            if (result.IsError)
            {
                return result;
            }

            await _uow.SaveChangesAsync(cancellationToken);
            await _redisCache.RemoveKeysByPattern(nameof(Expense));

            return Result<Empty>.Success();
        }
    }

    public sealed class DeleteExpenseCommandValidator : AbstractValidator<DeleteExpenseCommand>
    {
        public DeleteExpenseCommandValidator()
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
