namespace Application.Features.Queries;

public sealed record GetPaginatedCategoriesQuery : BaseRequest, IRequest<PaginatedList<CategoryDTO>>
{
    public string? Name { get; set; }
    public string? Keyword { get; set; }

    public sealed class GetPaginatedCategoriesQueryHandler(
        ICategoriesRepository repository,
        IMapper mapper,
        IRedisCache redisCache
    ) : IRequestHandler<GetPaginatedCategoriesQuery, PaginatedList<CategoryDTO>>
    {
        private readonly ICategoriesRepository _repository = repository;
        private readonly IMapper _mapper = mapper;
        private readonly IRedisCache _redisCache = redisCache;

        public async Task<PaginatedList<CategoryDTO>> Handle(
            GetPaginatedCategoriesQuery request,
            CancellationToken cancellationToken
        )
        {
            var redisKey =
                $"{nameof(Category)}#{request.Name}#{request.Keyword}#{request.Page}#{request.PageSize}";
            var cachedPaginatedCategories = await _redisCache.GetCachedData<
                PaginatedList<CategoryDTO>
            >(redisKey);
            if (cachedPaginatedCategories != null)
            {
                return cachedPaginatedCategories;
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
                DateTimeOffset.Now.AddMinutes(5)
            );

            return paginatedCategories;
        }
    }
}
