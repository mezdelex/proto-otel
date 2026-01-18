namespace Application.Features.Commands;

public sealed record DeleteCategoryCommand(string Id) : IRequest
{
    public sealed class DeleteCategoryCommandHandler(
        ICategoriesRepository repository,
        IUnitOfWork uow,
        IRedisCache redisCache
    ) : IRequestHandler<DeleteCategoryCommand>
    {
        private readonly ICategoriesRepository _repository = repository;
        private readonly IUnitOfWork _uow = uow;
        private readonly IRedisCache _redisCache = redisCache;

        public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            await _repository.DeleteAsync(request.Id, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            await _redisCache.RemoveKeysByPattern(nameof(Expense));
        }
    }

    public sealed class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
    {
        public DeleteCategoryCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage(GenericValidationMessages.ShouldNotBeEmpty(nameof(Id)))
                .MaximumLength(CategoryConstraints.IdMaxLength)
                .WithMessage(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(Id),
                        CategoryConstraints.IdMaxLength
                    )
                );
        }
    }
}
