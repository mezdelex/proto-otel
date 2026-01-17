namespace Presentation.Endpoints;

public static class ApplicationUsersEndpoints
{
    private static readonly ILogger _logger = new LoggerFactory().CreateLogger("ApplicationUsers");

    public static void MapApplicationUsersEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(MapGroups.ApplicationUsers);

        group.MapPost(Patterns.AllPattern, GetAllApplicationUsersQueryAsync).RequireAuthorization();
        group.MapGet(Patterns.IdPattern, GetApplicationUserQueryAsync).RequireAuthorization();
        group
            .MapGet(Patterns.LogoutPattern, LogoutApplicationUserQueryAsync)
            .RequireAuthorization();
    }

    public static async Task<IResult> GetAllApplicationUsersQueryAsync(
        ISender sender,
        [FromBody] GetAllApplicationUsersQuery query
    )
    {
        try
        {
            return Results.Ok(await sender.Send(query));
        }
        catch (Exception e)
        {
            _logger.LogError(Errors.ErrorMessageTemplate, e, e.Message);

            return Results.BadRequest(e.Message);
        }
    }

    public static async Task<IResult> GetApplicationUserQueryAsync(
        ISender sender,
        [FromRoute] string id
    )
    {
        try
        {
            return Results.Ok(await sender.Send(new GetApplicationUserQuery(id)));
        }
        catch (Exception e)
        {
            _logger.LogError(Errors.ErrorMessageTemplate, e, e.Message);

            return Results.NotFound(e.Message);
        }
    }

    public static async Task<IResult> LogoutApplicationUserQueryAsync(
        HttpContext context,
        ISender sender
    )
    {
        try
        {
            await context.SignOutAsync();

            return Results.NoContent();
        }
        catch (Exception e)
        {
            _logger.LogError(Errors.ErrorMessageTemplate, e, e.Message);

            return Results.BadRequest(e.Message);
        }
    }
}
