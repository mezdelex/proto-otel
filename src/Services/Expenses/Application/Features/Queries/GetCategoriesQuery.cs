namespace Application.Features.Queries;

public sealed record GetCategoriesQuery : IRequest<List<CategoryDTO>>
{
    public string? Name { get; set; }
    public string? ContainedWord { get; set; }

    public sealed class GetCategoriesQueryHandler(
        ICategoriesRepository repository,
        IMapper mapper,
        IRedisCache redisCache
    ) : IRequestHandler<GetCategoriesQuery, List<CategoryDTO>>
    {
        private readonly ICategoriesRepository _repository = repository;
        private readonly IMapper _mapper = mapper;
        private readonly IRedisCache _redisCache = redisCache;

        public async Task<List<CategoryDTO>> Handle(
            GetCategoriesQuery request,
            CancellationToken cancellationToken
        )
        {
            var redisKey = $"{nameof(Category)}#{request.Name}#{request.ContainedWord}";
            var cachedCategories = await _redisCache.GetCachedData<List<CategoryDTO>>(redisKey);
            if (cachedCategories != null)
            {
                return cachedCategories;
            }

            var paginatedCategories = await _repository
                .ApplySpecification(
                    new CategoriesSpecification(
                        name: request.Name,
                        containedWord: request.ContainedWord
                    )
                )
                .AsNoTracking()
                .ProjectTo<CategoryDTO>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            await _redisCache.SetCachedData(
                redisKey,
                paginatedCategories,
                DateTimeOffset.Now.AddMinutes(5)
            );

            return paginatedCategories;
        }
    }
}
