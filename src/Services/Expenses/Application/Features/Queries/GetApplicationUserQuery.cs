namespace Application.Features.Queries;

public sealed record GetApplicationUserQuery(string Id)
    : BaseRequest,
        IRequest<Result<ApplicationUserDTO>>
{
    public sealed class GetApplicationUserQueryHandler(
        IApplicationUsersRepository repository,
        IMapper mapper
    ) : IRequestHandler<GetApplicationUserQuery, Result<ApplicationUserDTO>>
    {
        private readonly IApplicationUsersRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<ApplicationUserDTO>> Handle(
            GetApplicationUserQuery request,
            CancellationToken cancellationToken
        )
        {
            var applicationUser = await _repository.GetBySpecAsync(
                new ApplicationUsersSpecification(id: request.Id),
                cancellationToken
            );
            if (applicationUser is null)
            {
                return Result<ApplicationUserDTO>.Failure(
                    new Error(
                        Errors.NotFoundError,
                        Errors.NotFoundErrorDetail(nameof(ApplicationUser)),
                        ErrorTypes.NotFound
                    )
                );
            }

            return Result<ApplicationUserDTO>.Success(
                _mapper.Map<ApplicationUserDTO>(applicationUser)
            );
        }
    }

    public class GetApplicationUserQueryValidator : AbstractValidator<GetApplicationUserQuery>
    {
        public GetApplicationUserQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage(GenericValidationMessages.ShouldNotBeEmpty(nameof(Id)))
                .MaximumLength(ApplicationUserConstraints.IdMaxLength)
                .WithMessage(
                    GenericValidationMessages.ShouldNotBeLongerThan(
                        nameof(Id),
                        ApplicationUserConstraints.IdMaxLength
                    )
                );
        }
    }
}
