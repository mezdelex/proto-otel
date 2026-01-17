namespace Presentation.Endpoints;

public static class CategoriesEndpoints
{
    private static readonly ILogger _logger = new LoggerFactory().CreateLogger("Categories");

    public static void MapCategoriesEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(MapGroups.Categories);

        group.MapPost(Patterns.AllPattern, GetAllCategoriesQueryAsync).RequireAuthorization();
        group.MapGet(Patterns.IdPattern, GetCategoryQueryAsync).RequireAuthorization();
        group.MapPatch(string.Empty, PatchCategoryCommandAsync).RequireAuthorization();
        group.MapPost(string.Empty, PostCategoryCommandAsync).RequireAuthorization();
        group.MapDelete(Patterns.IdPattern, DeleteCategoryCommandAsync).RequireAuthorization();
    }

    public static async Task<IResult> GetAllCategoriesQueryAsync(
        ISender sender,
        [FromBody] GetAllCategoriesQuery query
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

    public static async Task<IResult> GetCategoryQueryAsync(ISender sender, [FromRoute] string id)
    {
        try
        {
            return Results.Ok(await sender.Send(new GetCategoryQuery(id)));
        }
        catch (Exception e)
        {
            _logger.LogError(Errors.ErrorMessageTemplate, e, e.Message);

            return Results.NotFound(e.Message);
        }
    }

    public static async Task<IResult> PatchCategoryCommandAsync(
        ISender sender,
        [FromBody] PatchCategoryCommand command
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

    public static async Task<IResult> PostCategoryCommandAsync(
        ISender sender,
        [FromBody] PostCategoryCommand command
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

    public static async Task<IResult> DeleteCategoryCommandAsync(
        ISender sender,
        [FromRoute] string id
    )
    {
        try
        {
            await sender.Send(new DeleteCategoryCommand(id));

            return Results.NoContent();
        }
        catch (Exception e)
        {
            _logger.LogError(Errors.ErrorMessageTemplate, e, e.Message);

            return Results.BadRequest(e.Message);
        }
    }
}
