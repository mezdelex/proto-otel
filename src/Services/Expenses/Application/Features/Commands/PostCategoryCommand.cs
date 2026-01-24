namespace Application.Features.Commands;

public sealed record PostCategoryCommand(string Name, string Description) : IRequest<Result<Empty>>
{
    public sealed class PostCategoryCommandHandler(
        IMapper mapper,
        ICategoriesRepository repository,
        IUnitOfWork uow,
        IRedisCache redisCache,
        IEventBus eventBus
    ) : IRequestHandler<PostCategoryCommand, Result<Empty>>
    {
        private readonly IMapper _mapper = mapper;
        private readonly ICategoriesRepository _repository = repository;
        private readonly IUnitOfWork _uow = uow;
        private readonly IRedisCache _redisCache = redisCache;
        private readonly IEventBus _eventBus = eventBus;

        public async Task<Result<Empty>> Handle(
            PostCategoryCommand request,
            CancellationToken cancellationToken
        )
        {
            var categoryToPost = _mapper.Map<Category>(request);

            await _repository.PostAsync(categoryToPost, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            await Task.WhenAll([
                _redisCache.RemoveKeysByPattern(nameof(Category)),
                _eventBus.PublishAsync(
                    _mapper.Map<PostedCategoryEvent>(categoryToPost),
                    cancellationToken
                ),
                _eventBus.PublishAsync(
                    new NotificationEvent
                    {
                        EntityId = categoryToPost.Id,
                        EntityType = EntityType.Category,
                        Name = categoryToPost.Name,
                        Description = categoryToPost.Description,
                    },
                    cancellationToken
                ),
            ]);

            return Result<Empty>.Success();
        }
    }

    public sealed class PostCategoryCommandValidator : AbstractValidator<PostCategoryCommand>
    {
        public PostCategoryCommandValidator()
        {
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
