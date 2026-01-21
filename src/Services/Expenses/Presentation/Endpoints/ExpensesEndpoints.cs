namespace Presentation.Endpoints;

public static class ExpensesEndpoints
{
    private static readonly ILogger _logger = new LoggerFactory().CreateLogger("Expenses");

    public static void MapExpensesEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(MapGroups.Expenses);

        group
            .MapPost(Patterns.PaginatedPattern, GetPaginatedExpensesQueryAsync)
            .RequireAuthorization(nameof(Policies.UserRolePolicy));
        group
            .MapGet(Patterns.IdPattern, GetExpenseQueryAsync)
            .RequireAuthorization(nameof(Policies.UserRolePolicy));
        group
            .MapPatch(string.Empty, PatchExpenseCommandAsync)
            .RequireAuthorization(nameof(Policies.AdminRolePolicy));
        group
            .MapPost(string.Empty, PostExpenseCommandAsync)
            .RequireAuthorization(nameof(Policies.AdminRolePolicy));
        group
            .MapDelete(Patterns.IdPattern, DeleteExpenseCommandAsync)
            .RequireAuthorization(nameof(Policies.AdminRolePolicy));
    }

    public static async Task<IResult> GetPaginatedExpensesQueryAsync(
        ISender sender,
        [FromBody] GetPaginatedExpensesQuery query
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

    public static async Task<IResult> GetExpenseQueryAsync(ISender sender, [FromRoute] string id)
    {
        try
        {
            return Results.Ok(await sender.Send(new GetExpenseQuery(id)));
        }
        catch (Exception e)
        {
            _logger.LogError(Errors.ErrorMessageTemplate, e, e.Message);

            return Results.NotFound(e.Message);
        }
    }

    public static async Task<IResult> PatchExpenseCommandAsync(
        ISender sender,
        [FromBody] PatchExpenseCommand command
    )
    {
        try
        {
            await sender.Send(command);

            return Results.NoContent();
        }
        catch (Exception e)
        {
            _logger.LogError(Errors.ErrorMessageTemplate, e, e.Message);

            return Results.BadRequest(e.Message);
        }
    }

    public static async Task<IResult> PostExpenseCommandAsync(
        ISender sender,
        [FromBody] PostExpenseCommand command
    )
    {
        try
        {
            await sender.Send(command);

            return Results.NoContent();
        }
        catch (Exception e)
        {
            _logger.LogError(Errors.ErrorMessageTemplate, e, e.Message);

            return Results.BadRequest(e.Message);
        }
    }

    public static async Task<IResult> DeleteExpenseCommandAsync(
        ISender sender,
        [FromRoute] string id
    )
    {
        try
        {
            await sender.Send(new DeleteExpenseCommand(id));

            return Results.NoContent();
        }
        catch (Exception e)
        {
            _logger.LogError(Errors.ErrorMessageTemplate, e, e.Message);

            return Results.BadRequest(e.Message);
        }
    }
}
