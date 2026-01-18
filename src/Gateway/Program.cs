var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        Constants.DefaultCorsPolicy,
        cpb =>
        {
            cpb.WithOrigins("http://localhost:4200")
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
    .AddTransforms(tbc =>
    {
        tbc.AddRequestTransform(async rtc =>
        {
            if (rtc.HttpContext.User.Identity?.IsAuthenticated == true)
            {
                rtc.ProxyRequest.Headers.Authorization = new(
                    "Bearer",
                    TokenUtils.CreateGatewayToken(rtc.HttpContext.User, builder.Configuration)
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
    .ConfigureResource(rb => rb.AddService("gateway"))
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

app.UseCors(Constants.DefaultCorsPolicy);
app.UseAuthentication();
app.MapReverseProxy();

app.MapOpenApi();

app.Run();
