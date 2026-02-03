namespace Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResult
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var requestName = typeof(TRequest).Name;

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(
                "Starting request {RequestName}: {@Request}",
                requestName,
                request
            );
        }

        var stopwatch = Stopwatch.StartNew();
        var response = await next(cancellationToken);
        stopwatch.Stop();

        if (response.IsError)
        {
            logger.LogError(
                message: "Request {RequestName} failed after {Elapsed}ms. Error: {@Error}",
                args: [requestName, stopwatch.ElapsedMilliseconds, response.Error]
            );
        }
        else
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(
                    "Request {RequestName} succeeded after {Elapsed}ms",
                    requestName,
                    stopwatch.ElapsedMilliseconds
                );
            }
        }

        return response;
    }
}
