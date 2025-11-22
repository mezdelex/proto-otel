var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IEventBus, RabbitMQEventBus>();

builder.Services.AddMassTransit(busRegistrationConfigurator =>
{
    busRegistrationConfigurator.SetKebabCaseEndpointNameFormatter();
    busRegistrationConfigurator.AddConsumer<NotificationEventConsumer>();
    busRegistrationConfigurator.UsingRabbitMq(
        (busRegistrationContext, rabbitMQBusFactoryConfigurator) =>
        {
            rabbitMQBusFactoryConfigurator.Host(
                new Uri("amqp://rabbitmq"),
                rabbitMQHostConfigurator =>
                {
                    rabbitMQHostConfigurator.Username(builder.Configuration["USERNAME"]!);
                    rabbitMQHostConfigurator.Password(builder.Configuration["PASSWORD"]!);
                }
            );
            rabbitMQBusFactoryConfigurator.ConfigureEndpoints(busRegistrationContext);
        }
    );
});
builder
    .Services.AddOpenTelemetry()
    .ConfigureResource(rb => rb.AddService("notifications"))
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
            .AddHttpClientInstrumentation()
            .AddSource(DiagnosticHeaders.DefaultListenerName)
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri("http://otel-collector:4317");
                options.Protocol = OtlpExportProtocol.Grpc;
            })
    );

builder.Host.UseSerilog(
    (context, configuration) => configuration.ReadFrom.Configuration(context.Configuration)
);

var app = builder.Build();

app.UseSerilogRequestLogging();

app.Run();
