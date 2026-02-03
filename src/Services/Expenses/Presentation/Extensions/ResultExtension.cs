namespace Presentation.Extensions;

public static class ResultExtension
{
    public static Microsoft.AspNetCore.Http.IResult ToResponse<T>(this Result<T> result) =>
        result.Match(
            value =>
                value switch
                {
                    Empty => Results.NoContent(),
                    _ => Results.Ok(value),
                },
            error =>
                Results.Problem(
                    detail: error.Description,
                    statusCode: error.Type switch
                    {
                        ErrorTypes.Conflict => StatusCodes.Status409Conflict,
                        ErrorTypes.NotFound => StatusCodes.Status404NotFound,
                        ErrorTypes.Validation => StatusCodes.Status400BadRequest,
                        _ => StatusCodes.Status500InternalServerError,
                    },
                    title: error.Code
                )
        );
}
