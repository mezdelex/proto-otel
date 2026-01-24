namespace Application.Features.Commands;

public sealed record PatchCategoryCommand(string Id, string Name, string Description)
    : IRequest<Result<Empty>>
{
    public sealed class PatchCategoryCommandHandler(
        IMapper mapper,
        ICategoriesRepository repository,
        IUnitOfWork uow,
        IRedisCache redisCache,
        IEventBus eventBus
    ) : IRequestHandler<PatchCategoryCommand, Result<Empty>>
    {
        private readonly IMapper _mapper = mapper;
        private readonly ICategoriesRepository _repository = repository;
        private readonly IUnitOfWork _uow = uow;
        private readonly IRedisCache _redisCache = redisCache;
        private readonly IEventBus _eventBus = eventBus;

        public async Task<Result<Empty>> Handle(
            PatchCategoryCommand request,
            CancellationToken cancellationToken
        )
        {
            var categoryToPatch = _mapper.Map<Category>(request);

            var result = await _repository.PatchAsync(categoryToPatch, cancellationToken);
            if (result.IsError)
            {
                return result;
            }

            await _uow.SaveChangesAsync(cancellationToken);
            await Task.WhenAll([
                _redisCache.RemoveKeysByPattern(nameof(Category)),
                _eventBus.PublishAsync(
                    _mapper.Map<PatchedCategoryEvent>(categoryToPatch),
                    cancellationToken
                ),
            ]);

            return Result<Empty>.Success();
        }
    }

    public sealed class PatchCategoryCommandValidator : AbstractValidator<PatchCategoryCommand>
    {
        public PatchCategoryCommandValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty()
                .WithMessage(GenericValidationMessages.ShouldNotBeEmpty(nameof(Id)))
                .MaximumLength(CategoryConstraints.IdMaxLength)
                .WithMessage(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(Id),
                        CategoryConstraints.IdMaxLength
                    )
                );

            RuleFor(c => c.Name)
                .NotEmpty()
                .WithMessage(GenericValidationMessages.ShouldNotBeEmpty(nameof(Name)))
                .MaximumLength(CategoryConstraints.NameMaxLength)
                .WithMessage(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(Name),
                        CategoryConstraints.NameMaxLength
                    )
                );

            RuleFor(c => c.Description)
                .NotEmpty()
                .WithMessage(GenericValidationMessages.ShouldNotBeEmpty(nameof(Description)))
                .MaximumLength(CategoryConstraints.DescriptionMaxLength)
                .WithMessage(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(Description),
                        CategoryConstraints.DescriptionMaxLength
                    )
                );
        }
    }
}
