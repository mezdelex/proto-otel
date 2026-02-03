namespace Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResult
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var validationFailure = (
            await Task.WhenAll(validators.Select(x => x.ValidateAsync(request, cancellationToken)))
        )
            .Where(x => !x.IsValid)
            .SelectMany(x => x.Errors)
            .FirstOrDefault();

        return validationFailure is null
            ? await next(cancellationToken)
            : CreateValidationResult(validationFailure);
    }

    private static TResponse CreateValidationResult(ValidationFailure failure) =>
        (TResponse)
            typeof(TResponse)
                .GetMethod(nameof(Result<>.Failure), [typeof(Error)])!
                .Invoke(
                    null,
                    [new Error(failure.ErrorCode, failure.ErrorMessage, ErrorTypes.Validation)]
                )!;
}
