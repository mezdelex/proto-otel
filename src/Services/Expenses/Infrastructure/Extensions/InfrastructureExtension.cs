namespace Infrastructure.Extensions;

public static class InfrastructureExtension
{
    public static void AddInfrastructureDependencies(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                $"Server=sqlserver;Database={configuration["DATABASE"]};User Id=sa;Password={configuration["PASSWORD"]};TrustServerCertificate=True"
            )
        );
        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>()
        );
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect($"redis,password={configuration["PASSWORD"]}")
        );
        services.AddScoped(provider =>
            provider.GetRequiredService<IConnectionMultiplexer>().GetDatabase()
        );
        services.AddScoped<IEventBus, RabbitMQEventBus>();
        services.AddScoped<IRedisCache, RedisCache>();
        services.AddScoped<ISpecificationEvaluator>(provider => new SpecificationEvaluator(true));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddMassTransit(busRegistrationConfigurator =>
        {
            busRegistrationConfigurator.SetKebabCaseEndpointNameFormatter();
            busRegistrationConfigurator.AddConsumer<PatchedCategoryEventConsumer>();
            busRegistrationConfigurator.AddConsumer<PatchedExpenseEventConsumer>();
            busRegistrationConfigurator.AddConsumer<PostedCategoryEventConsumer>();
            busRegistrationConfigurator.AddConsumer<PostedExpenseEventConsumer>();
            busRegistrationConfigurator.UsingRabbitMq(
                (busRegistrationContext, rabbitMQBusFactoryConfigurator) =>
                {
                    rabbitMQBusFactoryConfigurator.Host(
                        new Uri("amqp://rabbitmq"),
                        rabbitMQHostConfigurator =>
                        {
                            rabbitMQHostConfigurator.Username(configuration["USERNAME"]!);
                            rabbitMQHostConfigurator.Password(configuration["PASSWORD"]!);
                        }
                    );
                    rabbitMQBusFactoryConfigurator.ConfigureEndpoints(busRegistrationContext);
                }
            );
        });
        services
            .AddOpenTelemetry()
            .ConfigureResource(rb => rb.AddService("expenses"))
            .WithMetrics(mpb =>
                mpb.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri("http://otel-collector:4317");
                        options.Protocol = OtlpExportProtocol.Grpc;
                    })
            )
            .WithTracing(tpb =>
                tpb.AddAspNetCoreInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRedisInstrumentation()
                    .AddSource(DiagnosticHeaders.DefaultListenerName)
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri("http://otel-collector:4317");
                        options.Protocol = OtlpExportProtocol.Grpc;
                    })
            );

        services.AddScoped<IApplicationUsersRepository, ApplicationUsersRepository>();
        services.AddScoped<ICategoriesRepository, CategoriesRepository>();
        services.AddScoped<IExpensesRepository, ExpensesRepository>();
    }
}
