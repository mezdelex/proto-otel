namespace Presentation.Endpoints;

public static class ExpensesEndpoints
{
    public static void MapExpensesEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(MapGroups.Expenses);
        var adminGroup = group.RequireAuthorization(nameof(Policies.AdminRolePolicy));
        var userGroup = group.RequireAuthorization(nameof(Policies.UserRolePolicy));

        userGroup.MapPost(
            Patterns.PaginatedPattern,
            async (ISender s, GetPaginatedExpensesQuery q) => (await s.Send(q)).ToResponse()
        );

        userGroup.MapPost(
            Patterns.ListPattern,
            async (ISender s, GetExpenseQuery q) => (await s.Send(q)).ToResponse()
        );

        userGroup.MapGet(
            Patterns.IdPattern,
            async (ISender s, string id) => (await s.Send(new GetExpenseQuery(id))).ToResponse()
        );

        adminGroup.MapPatch(
            string.Empty,
            async (ISender s, PatchExpenseCommand c) => (await s.Send(c)).ToResponse()
        );

        adminGroup.MapPost(
            string.Empty,
            async (ISender s, PostExpenseCommand c) => (await s.Send(c)).ToResponse()
        );

        adminGroup.MapDelete(
            Patterns.IdPattern,
            async (ISender s, string id) =>
                (await s.Send(new DeleteExpenseCommand(id))).ToResponse()
        );
    }
}
