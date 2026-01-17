var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder
    .Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddCookie(IdentityConstants.ApplicationScheme)
    .AddBearerToken(IdentityConstants.BearerScheme);
builder.Services.AddAuthorization(options =>
{
    var policy = new AuthorizationPolicyBuilder(
        IdentityConstants.ApplicationScheme,
        IdentityConstants.BearerScheme
    )
        .RequireAuthenticatedUser()
        .Build();

    options.DefaultPolicy = policy;
});
builder
    .Services.AddIdentityCore<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddApiEndpoints();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddInfrastructureDependencies(builder.Configuration);
builder.Services.AddApplicationDependencies();
builder.Services.AddPresentationDependencies();

builder.Host.UseSerilog(
    (context, configuration) => configuration.ReadFrom.Configuration(context.Configuration)
);

var app = builder.Build();

app.ApplyMigrations();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.MapGroup("/api/identity").MapIdentityApi<ApplicationUser>();

app.MapCategoriesEndpoints();
app.MapExpensesEndpoints();
app.MapApplicationUsersEndpoints();

app.UseSerilogRequestLogging();

app.Run();
