namespace Infrastructure.Interceptors;

public class AuditingInterceptor(IHttpContextAccessor httpContextAccessor) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData dbContextEventData,
        InterceptionResult<int> interceptionResult
    )
    {
        ApplyAuditing(dbContextEventData.Context);

        return base.SavingChanges(dbContextEventData, interceptionResult);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData dbContextEventData,
        InterceptionResult<int> interceptionResult,
        CancellationToken cancellationToken = default
    )
    {
        ApplyAuditing(dbContextEventData.Context);

        return base.SavingChangesAsync(dbContextEventData, interceptionResult, cancellationToken);
    }

    private void ApplyAuditing(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var now = DateTime.UtcNow;
        var email = httpContextAccessor.HttpContext?.User.Identity?.Name ?? "system@proto-otel.com";

        foreach (var entry in context.ChangeTracker.Entries<AuditEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = entry.Entity.ModifiedBy = email;
                entry.Entity.CreatedOn = entry.Entity.ModifiedOn = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(nameof(AuditEntity.CreatedBy)).IsModified = false;
                entry.Property(nameof(AuditEntity.CreatedOn)).IsModified = false;
                entry.Entity.ModifiedBy = email;
                entry.Entity.ModifiedOn = now;
            }
        }
    }
}
