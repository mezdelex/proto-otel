var builder = WebApplication.CreateBuilder(args);

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
