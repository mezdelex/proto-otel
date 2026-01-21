namespace Application.Features.Queries;

public sealed record GetPaginatedApplicationUsersQuery
    : BaseRequest,
        IRequest<PaginatedList<ApplicationUserDTO>>
{
    public string? Email { get; set; }
    public string? ContainedWord { get; set; }

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
            _repository.SetAsNoTracking();

            var redisKey =
                $"{nameof(ApplicationUser)}#{request.Email}#{request.ContainedWord}#{request.Page}#{request.PageSize}";
            var cachedPaginatedApplicationUsers = await _redisCache.GetCachedData<
                PaginatedList<ApplicationUserDTO>
            >(redisKey);
            if (cachedPaginatedApplicationUsers != null)
                return cachedPaginatedApplicationUsers;

            var paginatedApplicationUsers = await _repository
                .ApplySpecification(
                    new ApplicationUsersSpecification(
                        email: request.Email,
                        containedWord: request.ContainedWord
                    )
                )
                .Select(au => _mapper.Map<ApplicationUserDTO>(au))
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
