namespace Application.Features.Commands;

public sealed record PostExpenseCommand(
    string Name,
    string Description,
    decimal Value,
    string CategoryId
) : IRequest<Result<Empty>>
{
    public sealed class PostExpenseCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        IApplicationUsersRepository applicationUsersRepository,
        IMapper mapper,
        IExpensesRepository repository,
        IUnitOfWork uow,
        IRedisCache redisCache,
        IEventBus eventBus
    ) : IRequestHandler<PostExpenseCommand, Result<Empty>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IApplicationUsersRepository _applicationUsersRepository =
            applicationUsersRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IExpensesRepository _repository = repository;
        private readonly IUnitOfWork _uow = uow;
        private readonly IRedisCache _redisCache = redisCache;
        private readonly IEventBus _eventBus = eventBus;

        public async Task<Result<Empty>> Handle(
            PostExpenseCommand request,
            CancellationToken cancellationToken
        )
        {
            var email = _httpContextAccessor.HttpContext.User.Identity?.Name;
            var user = await _applicationUsersRepository.GetBySpecAsync(
                new ApplicationUsersSpecification(email: email),
                cancellationToken
            );
            if (user is null)
            {
                return Result<Empty>.Error([
                    new Error(
                        Errors.NotFoundError,
                        Errors.NotFoundErrorDetail(nameof(ApplicationUser)),
                        ErrorTypes.NotFound
                    ),
                ]);
            }

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

            return Result<Empty>.Success();
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
