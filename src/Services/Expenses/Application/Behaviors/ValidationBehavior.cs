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
        if (!validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            validators.Select(x => x.ValidateAsync(context, cancellationToken))
        );
        var failures = validationResults
            .SelectMany(x => x.Errors)
            .Where(validationFailure => validationFailure != null)
            .ToList();
        if (failures.Count != 0)
        {
            return CreateValidationResult<TResponse>(failures);
        }

        return await next(cancellationToken);
    }

    private static TResponse CreateValidationResult<T>(List<ValidationFailure> failures)
    {
        var errors = failures
            .Select(x => new Error(x.ErrorCode, x.ErrorMessage, ErrorTypes.Validation))
            .ToList();

        return (TResponse)
            typeof(T).GetMethod(nameof(Error), [typeof(List<Error>)])!.Invoke(null, [errors])!;
    }
}
