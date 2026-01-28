namespace Infrastructure.Extensions;

public static class InfrastructureExtension
{
    public static void AddInfrastructureDependencies(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddScoped<AuditingInterceptor>();
        services.AddDbContext<ApplicationDbContext>(
            (serviceProvider, dbContextOptionsBuilder) =>
                dbContextOptionsBuilder
                    .AddInterceptors(serviceProvider.GetRequiredService<AuditingInterceptor>())
                    .UseSqlServer(
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
        services.AddScoped<ISpecificationEvaluator>(provider => new SpecificationEvaluator());
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
        services
            .AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddBearerToken(IdentityConstants.BearerScheme)
            .AddCookie(IdentityConstants.ApplicationScheme)
            .AddJwtBearer(jbo =>
            {
                jbo.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["CLIENT_SECRET"] ?? string.Empty)
                    ),
                    ValidAudiences = [configuration["CLIENT_ID"]],
                    ValidIssuer = configuration["ISSUER"],
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                };
            });
        services.AddAuthorization(options =>
        {
            var defaultPolicy = new AuthorizationPolicyBuilder(
                IdentityConstants.ApplicationScheme,
                IdentityConstants.BearerScheme
            )
                .RequireAuthenticatedUser()
                .Build();

            options.AddPolicy(
                nameof(Policies.AdminRolePolicy),
                new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .RequireRole(nameof(Roles.Admin))
                    .Build()
            );

            options.AddPolicy(
                nameof(Policies.UserRolePolicy),
                new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .RequireRole([nameof(Roles.Admin), nameof(Roles.User)])
                    .Build()
            );

            options.DefaultPolicy = defaultPolicy;
        });
        services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>>()
            .AddApiEndpoints();
        services
            .AddDataProtection()
            .PersistKeysToFileSystem(
                new DirectoryInfo(configuration["APP_DATA_PROTECTION_KEY_PATH"] ?? string.Empty)
            )
            .SetApplicationName(nameof(DataProtectionProvider));
        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.None;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });

        services.AddScoped<IApplicationUsersRepository, ApplicationUsersRepository>();
        services.AddScoped<ICategoriesRepository, CategoriesRepository>();
        services.AddScoped<IExpensesRepository, ExpensesRepository>();
    }
}
