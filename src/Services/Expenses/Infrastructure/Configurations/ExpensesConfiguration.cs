namespace Infrastructure.Configurations;

public class ExpensesConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(ExpenseConstraints.NameMaxLength).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(ExpenseConstraints.DescriptionMaxLength);
        builder
            .Property(x => x.Value)
            .HasPrecision(BaseConstraints.DecimalPrecision, BaseConstraints.DecimalScale)
            .IsRequired();
        builder.Property(x => x.Date).HasPrecision(BaseConstraints.DatePrecision).IsRequired();
        builder.ConfigureAuditingValues();

        builder
            .HasOne(x => x.Category)
            .WithMany(c => c.Expenses)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        builder
            .HasOne(x => x.ApplicationUser)
            .WithMany(au => au.Expenses)
            .HasForeignKey(x => x.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
