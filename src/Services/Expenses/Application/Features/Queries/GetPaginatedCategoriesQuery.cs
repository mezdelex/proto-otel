namespace Application.Features.Queries;

public sealed record GetPaginatedCategoriesQuery : BaseRequest, IRequest<PaginatedList<CategoryDTO>>
{
    public string? Name { get; set; }
    public string? ContainedWord { get; set; }

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
            _repository.SetAsNoTracking();

            var redisKey =
                $"{nameof(Category)}#{request.Name}#{request.ContainedWord}#{request.Page}#{request.PageSize}";
            var cachedPaginatedCategories = await _redisCache.GetCachedData<
                PaginatedList<CategoryDTO>
            >(redisKey);
            if (cachedPaginatedCategories != null)
                return cachedPaginatedCategories;

            var paginatedCategories = await _repository
                .ApplySpecification(
                    new CategoriesSpecification(
                        name: request.Name,
                        containedWord: request.ContainedWord
                    )
                )
                .Select(c => _mapper.Map<CategoryDTO>(c))
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
