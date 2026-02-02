var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(corsOptions =>
{
    corsOptions.AddPolicy(
        Constants.DefaultCorsPolicy,
        corsPolicyBuilder =>
        {
            corsPolicyBuilder
                .WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    );
});
builder
    .Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddCookie(IdentityConstants.ApplicationScheme);
builder
    .Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(transformBuilderContext =>
    {
        transformBuilderContext.AddRequestTransform(async requestTransformContext =>
        {
            if (requestTransformContext.HttpContext.User.Identity?.IsAuthenticated == true)
            {
                requestTransformContext.ProxyRequest.Headers.Authorization = new(
                    "Bearer",
                    TokenUtils.CreateGatewayToken(
                        requestTransformContext.HttpContext.User,
                        builder.Configuration
                    )
                );
            }
        });
    });
builder
    .Services.AddDataProtection()
    .PersistKeysToFileSystem(
        new DirectoryInfo(builder.Configuration["APP_DATA_PROTECTION_KEY_PATH"] ?? string.Empty)
    )
    .SetApplicationName(nameof(DataProtectionProvider));

builder.Services.AddOpenApi();

builder
    .Services.AddOpenTelemetry()
    .ConfigureResource(resourceBuilder => resourceBuilder.AddService("gateway"))
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

app.UseCors(Constants.DefaultCorsPolicy);
app.UseAuthentication();
app.MapReverseProxy();

app.MapOpenApi();

app.Run();
