namespace Application.Features.Commands;

public sealed record PostExpenseCommand(
    string Name,
    string Description,
    decimal Value,
    string CategoryId
) : IRequest
{
    public sealed class PostExpenseCommandHandler(
        IValidator<PostExpenseCommand> validator,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IApplicationUsersRepository applicationUsersRepository,
        IExpensesRepository repository,
        IUnitOfWork uow,
        IRedisCache redisCache,
        IEventBus eventBus
    ) : IRequestHandler<PostExpenseCommand>
    {
        private readonly IValidator<PostExpenseCommand> _validator = validator;
        private readonly IMapper _mapper = mapper;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IApplicationUsersRepository _applicationUsersRepository =
            applicationUsersRepository;
        private readonly IExpensesRepository _repository = repository;
        private readonly IUnitOfWork _uow = uow;
        private readonly IRedisCache _redisCache = redisCache;
        private readonly IEventBus _eventBus = eventBus;

        public async Task Handle(PostExpenseCommand request, CancellationToken cancellationToken)
        {
            var results = await _validator.ValidateAsync(request, cancellationToken);
            if (!results.IsValid)
            {
                throw new ValidationException(results.ToString().Replace("\r\n", " "));
            }

            var email = _httpContextAccessor.HttpContext.User.Identity?.Name;
            var user =
                await _applicationUsersRepository.GetBySpecAsync(
                    new ApplicationUsersSpecification(email: email),
                    cancellationToken
                ) ?? throw new NotFoundException(nameof(ApplicationUser));

            var expenseToPost = _mapper.Map<Expense>(request);
            expenseToPost.ApplicationUserId = user.Id;

            await _repository.PostAsync(expenseToPost, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            await Task.WhenAll(
                _redisCache.RemoveKeysByPattern(nameof(Expense)),
                _eventBus.PublishAsync(
                    _mapper.Map<PostedExpenseEvent>(expenseToPost),
                    cancellationToken
                )
            );
        }
    }

    public sealed class PostExpenseCommandValidator : AbstractValidator<PostExpenseCommand>
    {
        public PostExpenseCommandValidator()
        {
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

            RuleFor(c => c.CategoryId)
                .NotEmpty()
                .WithMessage(GenericValidationMessages.ShouldNotBeEmpty(nameof(CategoryId)))
                .MaximumLength(CategoryConstraints.IdMaxLength)
                .WithMessage(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(CategoryId),
                        CategoryConstraints.IdMaxLength
                    )
                );
        }
    }
}
