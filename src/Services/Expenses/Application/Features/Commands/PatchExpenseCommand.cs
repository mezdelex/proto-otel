namespace Application.Features.Commands;

public sealed record PatchExpenseCommand(
    string Id,
    string Name,
    string Description,
    decimal Value,
    DateTimeOffset Date,
    string CategoryId,
    string ApplicationUserId
) : IRequest<Result<Empty>>
{
    public sealed class PatchExpenseCommandHandler(
        IMapper mapper,
        IExpensesRepository repository,
        IUnitOfWork uow,
        IRedisCache redisCache,
        IEventBus eventBus
    ) : IRequestHandler<PatchExpenseCommand, Result<Empty>>
    {
        private readonly IMapper _mapper = mapper;
        private readonly IExpensesRepository _repository = repository;
        private readonly IUnitOfWork _uow = uow;
        private readonly IRedisCache _redisCache = redisCache;
        private readonly IEventBus _eventBus = eventBus;

        public async Task<Result<Empty>> Handle(
            PatchExpenseCommand request,
            CancellationToken cancellationToken
        )
        {
            var expenseToPatch = _mapper.Map<Expense>(request);

            var result = await _repository.PatchAsync(expenseToPatch, cancellationToken);
            if (result.IsError)
            {
                return result;
            }

            await _uow.SaveChangesAsync(cancellationToken);
            await Task.WhenAll([
                _redisCache.RemoveKeysByTags(nameof(Expense)),
                _eventBus.PublishAsync(
                    _mapper.Map<PatchedExpenseEvent>(expenseToPatch),
                    cancellationToken
                ),
            ]);

            return Result<Empty>.Success();
        }
    }

    public sealed class PatchExpenseCommandValidator : AbstractValidator<PatchExpenseCommand>
    {
        public PatchExpenseCommandValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty()
                .WithMessage(GenericValidationMessages.ShouldNotBeEmpty(nameof(Id)))
                .MaximumLength(ExpenseConstraints.IdMaxLength)
                .WithMessage(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(Id),
                        ExpenseConstraints.IdMaxLength
                    )
                );

            RuleFor(c => c.Name)
                .NotEmpty()
                .WithMessage(GenericValidationMessages.ShouldNotBeEmpty(nameof(Name)))
                .MaximumLength(ExpenseConstraints.NameMaxLength)
                .WithMessage(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(Name),
                        ExpenseConstraints.NameMaxLength
                    )
                );

            RuleFor(c => c.Description)
                .NotEmpty()
                .WithMessage(GenericValidationMessages.ShouldNotBeEmpty(nameof(Description)))
                .MaximumLength(ExpenseConstraints.DescriptionMaxLength)
                .WithMessage(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(Description),
                        ExpenseConstraints.DescriptionMaxLength
                    )
                );

            RuleFor(c => c.Value)
                .NotEmpty()
                .WithMessage(GenericValidationMessages.ShouldNotBeEmpty(nameof(Value)));

            RuleFor(c => c.Date)
                .NotEmpty()
                .WithMessage(GenericValidationMessages.ShouldNotBeEmpty(nameof(Date)));

            RuleFor(c => c.CategoryId)
                .NotEmpty()
                .WithMessage(GenericValidationMessages.ShouldNotBeEmpty(nameof(CategoryId)))
                .MaximumLength(ExpenseConstraints.IdMaxLength)
                .WithMessage(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(CategoryId),
                        CategoryConstraints.IdMaxLength
                    )
                );

            RuleFor(c => c.ApplicationUserId)
                .NotEmpty()
                .WithMessage(GenericValidationMessages.ShouldNotBeEmpty(nameof(ApplicationUserId)))
                .MaximumLength(ApplicationUserConstraints.IdMaxLength)
                .WithMessage(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(ApplicationUserId),
                        ApplicationUserConstraints.IdMaxLength
                    )
                );
        }
    }
}
