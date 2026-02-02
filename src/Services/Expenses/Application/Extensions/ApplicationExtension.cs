namespace Application.Extensions;

public static class ApplicationExtension
{
    public static void AddApplicationDependencies(this IServiceCollection services)
    {
        var assembly = typeof(ApplicationExtension).Assembly;

        services.AddAutoMapper(mapperConfigurationExpression =>
            mapperConfigurationExpression.AddMaps(assembly)
        );
        services.AddMediatR(mediatRServiceConfiguration =>
        {
            mediatRServiceConfiguration.AddOpenBehavior(typeof(LoggingBehavior<,>));
            mediatRServiceConfiguration.AddOpenBehavior(typeof(ValidationBehavior<,>));
            mediatRServiceConfiguration.RegisterServicesFromAssembly(assembly);
        });
        services.AddValidatorsFromAssembly(assembly);
    }
}
