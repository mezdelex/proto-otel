namespace Infrastructure.Configurations;

public class CategoriesConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(CategoryConstraints.NameMaxLength).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(CategoryConstraints.DescriptionMaxLength);
        builder.ConfigureAuditingValues();

        builder.HasIndex(x => x.Name).IsUnique();
    }
}
