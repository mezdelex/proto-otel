namespace Infrastructure.Extensions;

public static class SeedingExtension
{
    public static void SeedData(this ModelBuilder builder)
    {
        builder
            .Entity<Category>()
            .HasData([
                new()
                {
                    Id = "f5a8a867-27b1-477d-8f9c-59e354964951",
                    Name = "Groceries",
                    Description = "Groceries category.",
                },
                new()
                {
                    Id = "a9c12b73-0f1c-4b34-8975-57321689104c",
                    Name = "Transportation",
                    Description = "Transportation category.",
                },
                new()
                {
                    Id = "8b3e5182-e3d4-42f5-8d91-384157d692a8",
                    Name = "Leisure",
                    Description = "Leisure category.",
                },
                new()
                {
                    Id = "d42f928e-671c-4972-881b-5e8396123924",
                    Name = "Utilities",
                    Description = "Utilities category.",
                },
            ]);

        builder
            .Entity<IdentityRole>()
            .HasData(
                new IdentityRole
                {
                    Id = "8f3e5182-e3d4-42f5-8d91-384157d692a8",
                    ConcurrencyStamp = "66e3f2ae-65ec-4190-83c8-0a34368684af",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                },
                new IdentityRole
                {
                    Id = "d52f928e-671c-4972-881b-5e8396123924",
                    ConcurrencyStamp = "b7e7298d-b916-42eb-94a1-1df59171087b",
                    Name = "User",
                    NormalizedName = "USER",
                }
            );
    }
}
