namespace Presentation.Endpoints;

public static class ExpensesEndpoints
{
    private static readonly ILogger _logger = new LoggerFactory().CreateLogger("Expenses");

    public static void MapExpensesEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(MapGroups.Expenses);

        group
            .MapPost(Patterns.AllPattern, GetAllExpensesQueryAsync)
            .RequireAuthorization(nameof(Policies.AdminRolePolicy));
        group.MapGet(Patterns.IdPattern, GetExpenseQueryAsync).RequireAuthorization();
        group.MapPatch(string.Empty, PatchExpenseCommandAsync).RequireAuthorization();
        group.MapPost(string.Empty, PostExpenseCommandAsync).RequireAuthorization();
        group
            .MapDelete(Patterns.IdPattern, DeleteExpenseCommandAsync)
            .RequireAuthorization(nameof(Policies.AdminRolePolicy));
    }

    public static async Task<IResult> GetAllExpensesQueryAsync(
        ISender sender,
        [FromBody] GetAllExpensesQuery query
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
