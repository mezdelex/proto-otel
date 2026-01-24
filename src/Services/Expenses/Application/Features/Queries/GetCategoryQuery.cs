namespace Application.Features.Queries;

public sealed record GetCategoryQuery(string Id) : IRequest<Result<CategoryDTO>>
{
    public sealed class GetCategoryQueryHandler(ICategoriesRepository repository, IMapper mapper)
        : IRequestHandler<GetCategoryQuery, Result<CategoryDTO>>
    {
        private readonly ICategoriesRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<CategoryDTO>> Handle(
            GetCategoryQuery request,
            CancellationToken cancellationToken
        )
        {
            var category = await _repository.GetBySpecAsync(
                new CategoriesSpecification(id: request.Id),
                cancellationToken
            );
            if (category is null)
            {
                return Result<CategoryDTO>.Error([
                    new Error(
                        Errors.NotFoundError,
                        Errors.NotFoundErrorDetail(nameof(Category)),
                        ErrorTypes.NotFound
                    ),
                ]);
            }

            return Result<CategoryDTO>.Success(_mapper.Map<CategoryDTO>(category));
        }
    }

    public class GetCategoryQueryValidator : AbstractValidator<GetCategoryQuery>
    {
        public GetCategoryQueryValidator()
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
