namespace Presentation.Endpoints;

public static class ApplicationUsersEndpoints
{
    public static void MapApplicationUsersEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(MapGroups.ApplicationUsers);
        var adminGroup = group.RequireAuthorization(nameof(Policies.AdminRolePolicy));
        var userGroup = group.RequireAuthorization(nameof(Policies.UserRolePolicy));

        adminGroup.MapPost(
            Patterns.PaginatedPattern,
            async (ISender s, GetPaginatedApplicationUsersQuery q) => (await s.Send(q)).ToResponse()
        );

        adminGroup.MapPost(
            Patterns.IdPattern,
            async (ISender s, string id) =>
                (await s.Send(new GetApplicationUserQuery(id))).ToResponse()
        );

        userGroup.MapGet(Patterns.LogoutPattern, async c => await c.SignOutAsync());
    }
}
