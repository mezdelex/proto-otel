namespace Application.Features.Queries;

public sealed record GetCategoriesQuery : IRequest<Result<List<CategoryDTO>>>
{
    public string? Name { get; set; }
    public string? Keyword { get; set; }

    public sealed class GetCategoriesQueryHandler(
        IRedisCache redisCache,
        ICategoriesRepository repository,
        IMapper mapper
    ) : IRequestHandler<GetCategoriesQuery, Result<List<CategoryDTO>>>
    {
        private readonly IRedisCache _redisCache = redisCache;
        private readonly ICategoriesRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<List<CategoryDTO>>> Handle(
            GetCategoriesQuery request,
            CancellationToken cancellationToken
        )
        {
            var redisKey = $"{nameof(Category)}#{request.Name}#{request.Keyword}";
            var cachedCategories = await _redisCache.GetCachedData<List<CategoryDTO>>(redisKey);
            if (cachedCategories != null)
            {
                return Result<List<CategoryDTO>>.Success(cachedCategories);
            }

            var categories = await _repository
                .ApplySpecification(
                    new CategoriesSpecification(name: request.Name, keyword: request.Keyword)
                )
                .AsNoTracking()
                .ProjectTo<CategoryDTO>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            await _redisCache.SetCachedData(redisKey, categories, DateTimeOffset.Now.AddMinutes(5));

            return Result<List<CategoryDTO>>.Success(categories);
        }
    }

    public class GetCategoriesQueryValidator : AbstractValidator<GetCategoriesQuery>
    {
        public GetCategoriesQueryValidator()
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
