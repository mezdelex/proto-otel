namespace Presentation.Handlers;

public sealed class GlobalExceptionHandler(ILogger logger) : IExceptionHandler
{
    private readonly ILogger _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        var problemDetails = new ProblemDetails
        {
            Detail = Errors.ServerErrorDetail,
            Status = StatusCodes.Status500InternalServerError,
            Title = Errors.ServerError,
        };
        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        _logger.LogError("{@ProblemDetails}", problemDetails);

        return true;
    }
}
