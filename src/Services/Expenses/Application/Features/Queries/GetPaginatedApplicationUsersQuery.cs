namespace Application.Features.Queries;

public sealed record GetPaginatedApplicationUsersQuery
    : BaseRequest,
        IRequest<PaginatedList<ApplicationUserDTO>>
{
    public string? Email { get; set; }
    public string? Keyword { get; set; }

    public sealed class GetPaginatedApplicationUsersQueryHandler(
        IApplicationUsersRepository repository,
        IMapper mapper,
        IRedisCache redisCache
    ) : IRequestHandler<GetPaginatedApplicationUsersQuery, PaginatedList<ApplicationUserDTO>>
    {
        private readonly IApplicationUsersRepository _repository = repository;
        private readonly IMapper _mapper = mapper;
        private readonly IRedisCache _redisCache = redisCache;

        public async Task<PaginatedList<ApplicationUserDTO>> Handle(
            GetPaginatedApplicationUsersQuery request,
            CancellationToken cancellationToken
        )
        {
            var redisKey =
                $"{nameof(ApplicationUser)}#{request.Email}#{request.Keyword}#{request.Page}#{request.PageSize}";
            var cachedPaginatedApplicationUsers = await _redisCache.GetCachedData<
                PaginatedList<ApplicationUserDTO>
            >(redisKey);
            if (cachedPaginatedApplicationUsers != null)
            {
                return cachedPaginatedApplicationUsers;
            }

            var paginatedApplicationUsers = await _repository
                .ApplySpecification(
                    new ApplicationUsersSpecification(
                        email: request.Email,
                        keyword: request.Keyword
                    )
                )
                .AsNoTracking()
                .ProjectTo<ApplicationUserDTO>(_mapper.ConfigurationProvider)
                .ToPaginatedListAsync(request.Page, request.PageSize, cancellationToken);

            await _redisCache.SetCachedData(
                redisKey,
                paginatedApplicationUsers,
                DateTimeOffset.Now.AddMinutes(5)
            );

            return paginatedApplicationUsers;
        }
    }
}
