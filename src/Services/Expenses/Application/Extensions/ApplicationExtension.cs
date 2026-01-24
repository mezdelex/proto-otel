namespace Application.Extensions;

public static class ApplicationExtension
{
    public static void AddApplicationDependencies(this IServiceCollection services)
    {
        var assembly = typeof(ApplicationExtension).Assembly;

        services.AddAutoMapper(mce => mce.AddMaps(assembly));
        services.AddMediatR(configuration =>
        {
            configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
            configuration.RegisterServicesFromAssembly(assembly);
        });
        services.AddValidatorsFromAssembly(assembly);
    }
}
