namespace Infrastructure.Extensions;

public static class SeedingExtension
{
    public static void SeedData(this ModelBuilder builder)
    {
        builder
            .Entity<Category>()
            .HasData(
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Groceries",
                    Description = "Groceries category.",
                },
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Transportation",
                    Description = "Transportation category.",
                },
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Leisure",
                    Description = "Leisure category.",
                },
                new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Utilities",
                    Description = "Utilities category.",
                }
            );
    }
}
