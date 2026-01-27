namespace Application.Features.Commands;

public sealed record DeleteCategoryCommand(string Id) : IRequest<Result<Empty>>
{
    public sealed class DeleteCategoryCommandHandler(
        ICategoriesRepository repository,
        IUnitOfWork uow,
        IRedisCache redisCache
    ) : IRequestHandler<DeleteCategoryCommand, Result<Empty>>
    {
        private readonly ICategoriesRepository _repository = repository;
        private readonly IUnitOfWork _uow = uow;
        private readonly IRedisCache _redisCache = redisCache;

        public async Task<Result<Empty>> Handle(
            DeleteCategoryCommand request,
            CancellationToken cancellationToken
        )
        {
            var result = await _repository.DeleteAsync(request.Id, cancellationToken);
            if (result.IsError)
            {
                return result;
            }

            await _uow.SaveChangesAsync(cancellationToken);
            await _redisCache.RemoveKeysByTags(nameof(Expense));

            return Result<Empty>.Success();
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
