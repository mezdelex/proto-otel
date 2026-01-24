namespace Application.Features.Queries;

public sealed record GetPaginatedApplicationUsersQuery
    : BaseRequest,
        IRequest<Result<PaginatedList<ApplicationUserDTO>>>
{
    public string? Email { get; set; }
    public string? Keyword { get; set; }

    public sealed class GetPaginatedApplicationUsersQueryHandler(
        IRedisCache redisCache,
        IApplicationUsersRepository repository,
        IMapper mapper
    )
        : IRequestHandler<
            GetPaginatedApplicationUsersQuery,
            Result<PaginatedList<ApplicationUserDTO>>
        >
    {
        private readonly IRedisCache _redisCache = redisCache;
        private readonly IApplicationUsersRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<PaginatedList<ApplicationUserDTO>>> Handle(
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
                return Result<PaginatedList<ApplicationUserDTO>>.Success(
                    cachedPaginatedApplicationUsers
                );
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

            return Result<PaginatedList<ApplicationUserDTO>>.Success(paginatedApplicationUsers);
        }
    }
}
