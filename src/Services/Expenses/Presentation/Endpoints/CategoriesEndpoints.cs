namespace Presentation.Endpoints;

public static class CategoriesEndpoints
{
    private static readonly ILogger _logger = new LoggerFactory().CreateLogger("Categories");

    public static void MapCategoriesEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(MapGroups.Categories);

        group
            .MapPost(Patterns.PaginatedPattern, GetPaginatedCategoriesQueryAsync)
            .RequireAuthorization(nameof(Policies.UserRolePolicy));
        group
            .MapPost(Patterns.ListPattern, GetCategoriesQueryAsync)
            .RequireAuthorization(nameof(Policies.UserRolePolicy));
        group
            .MapGet(Patterns.IdPattern, GetCategoryQueryAsync)
            .RequireAuthorization(nameof(Policies.UserRolePolicy));
        group
            .MapPatch(string.Empty, PatchCategoryCommandAsync)
            .RequireAuthorization(nameof(Policies.AdminRolePolicy));
        group
            .MapPost(string.Empty, PostCategoryCommandAsync)
            .RequireAuthorization(nameof(Policies.AdminRolePolicy));
        group
            .MapDelete(Patterns.IdPattern, DeleteCategoryCommandAsync)
            .RequireAuthorization(nameof(Policies.AdminRolePolicy));
    }

    public static async Task<IResult> GetPaginatedCategoriesQueryAsync(
        ISender sender,
        [FromBody] GetPaginatedCategoriesQuery query
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

    public static async Task<IResult> GetCategoriesQueryAsync(
        ISender sender,
        [FromBody] GetCategoriesQuery query
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
