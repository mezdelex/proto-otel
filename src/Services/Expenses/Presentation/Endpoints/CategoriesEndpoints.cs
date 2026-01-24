namespace Presentation.Endpoints;

public static class CategoriesEndpoints
{
    public static void MapCategoriesEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(MapGroups.Categories);
        var adminGroup = group.RequireAuthorization(nameof(Policies.AdminRolePolicy));
        var userGroup = group.RequireAuthorization(nameof(Policies.UserRolePolicy));

        userGroup.MapPost(
            Patterns.PaginatedPattern,
            async (ISender s, GetPaginatedCategoriesQuery q) => (await s.Send(q)).ToResponse()
        );

        userGroup.MapPost(
            Patterns.ListPattern,
            async (ISender s, GetCategoriesQuery q) => (await s.Send(q)).ToResponse()
        );

        userGroup.MapGet(
            Patterns.IdPattern,
            async (ISender s, string id) => (await s.Send(new GetCategoryQuery(id))).ToResponse()
        );

        adminGroup.MapPatch(
            string.Empty,
            async (ISender s, PatchCategoryCommand c) => (await s.Send(c)).ToResponse()
        );

        adminGroup.MapPost(
            string.Empty,
            async (ISender s, PostCategoryCommand c) => (await s.Send(c)).ToResponse()
        );

        adminGroup.MapDelete(
            Patterns.IdPattern,
            async (ISender s, string id) =>
                (await s.Send(new DeleteCategoryCommand(id))).ToResponse()
        );
    }
}
