namespace Application.Features.Queries;

public sealed record GetPaginatedCategoriesQuery
    : BaseRequest,
        IRequest<Result<PaginatedList<CategoryDTO>>>
{
    public string? Name { get; set; }
    public string? Keyword { get; set; }

    public sealed class GetPaginatedCategoriesQueryHandler(
        IRedisCache redisCache,
        ICategoriesRepository repository,
        IMapper mapper
    ) : IRequestHandler<GetPaginatedCategoriesQuery, Result<PaginatedList<CategoryDTO>>>
    {
        private readonly IRedisCache _redisCache = redisCache;
        private readonly ICategoriesRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<PaginatedList<CategoryDTO>>> Handle(
            GetPaginatedCategoriesQuery request,
            CancellationToken cancellationToken
        )
        {
            var redisKey = _redisCache.GenerateKey(
                request.Name,
                request.Keyword,
                request.Page,
                request.PageSize
            );
            var cachedPaginatedCategories = await _redisCache.GetCachedData<
                PaginatedList<CategoryDTO>
            >(redisKey);
            if (cachedPaginatedCategories != null)
            {
                return Result<PaginatedList<CategoryDTO>>.Success(cachedPaginatedCategories);
            }

            var paginatedCategories = await _repository
                .ApplySpecification(
                    new CategoriesSpecification(name: request.Name, keyword: request.Keyword)
                )
                .AsNoTracking()
                .ProjectTo<CategoryDTO>(_mapper.ConfigurationProvider)
                .ToPaginatedListAsync(request.Page, request.PageSize, cancellationToken);

            await _redisCache.SetCachedData(
                redisKey,
                paginatedCategories,
                TimeSpan.FromMinutes(5),
                nameof(Category)
            );

            return Result<PaginatedList<CategoryDTO>>.Success(paginatedCategories);
        }
    }

    public class GetPaginatedCategoriesQueryValidator
        : AbstractValidator<GetPaginatedCategoriesQuery>
    {
        public GetPaginatedCategoriesQueryValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(CategoryConstraints.NameMaxLength)
                .WithMessage(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(Name),
                        CategoryConstraints.NameMaxLength
                    )
                )
                .When(x => !string.IsNullOrWhiteSpace(x.Name));

            RuleFor(x => x.Keyword)
                .MaximumLength(CategoryConstraints.DescriptionMaxLength)
                .WithMessage(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(Keyword),
                        CategoryConstraints.DescriptionMaxLength
                    )
                )
                .When(x => !string.IsNullOrWhiteSpace(x.Keyword));
        }
    }
}
