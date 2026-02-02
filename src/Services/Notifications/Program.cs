var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    .ConfigureResource(resourceBuilder => resourceBuilder.AddService("notifications"))
    .WithMetrics(meterProviderBuilder =>
        meterProviderBuilder
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddOtlpExporter(otlpExporterOptions =>
            {
                otlpExporterOptions.Endpoint = new Uri("http://otel-collector:4317");
                otlpExporterOptions.Protocol = OtlpExportProtocol.Grpc;
            })
    )
    .WithTracing(tracerProviderBuilder =>
        tracerProviderBuilder
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSource(DiagnosticHeaders.DefaultListenerName)
            .AddOtlpExporter(otlpExporterOptions =>
            {
                otlpExporterOptions.Endpoint = new Uri("http://otel-collector:4317");
                otlpExporterOptions.Protocol = OtlpExportProtocol.Grpc;
            })
    );

builder.Host.UseSerilog(
    (hostBuilderContext, loggerConfiguration) =>
        loggerConfiguration.ReadFrom.Configuration(hostBuilderContext.Configuration)
);

var app = builder.Build();

app.UseSerilogRequestLogging();

app.Run();
