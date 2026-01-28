namespace Infrastructure.Extensions;

public static class AuditingExtension
{
    public static void ConfigureAuditingValues<T>(this EntityTypeBuilder<T> builder)
        where T : AuditEntity
    {
        builder.Property(x => x.CreatedBy).IsRequired();
        builder.Property(x => x.CreatedOn).HasPrecision(BaseConstraints.DatePrecision).IsRequired();
        builder.Property(x => x.ModifiedBy).IsRequired();
        builder
            .Property(x => x.ModifiedOn)
            .HasPrecision(BaseConstraints.DatePrecision)
            .IsRequired();
    }
}
