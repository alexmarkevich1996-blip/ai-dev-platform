using FluentValidation;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Npgsql;
using Platform.Accounts.Domain;
using Platform.Accounts.Infrastructure;
using Platform.Accounts.Infrastructure.Persistence;
using Platform.AgentDelivery;
using Platform.Host.Endpoints;
using Platform.Host.Health;
using Platform.Host.Startup;
using Platform.Integrations;
using Platform.Projects.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Structured logging to the console only. Seq is deferred as observability work.
builder.Services.AddSerilog((_, configuration) => configuration
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console());

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException(
        "No database connection string. Set ConnectionStrings__Default; see .env.example.");

// One data source for the readiness check, separate from the module contexts so
// that the check tests the connection rather than Entity Framework Core's state.
builder.Services.AddSingleton(_ => new NpgsqlDataSourceBuilder(connectionString).Build());

// The modules. Each one owns its schema; the composition root is the only place
// that knows they share a database.
builder.Services.AddAccountsInfrastructure(connectionString);
builder.Services.AddProjectsInfrastructure(connectionString);
builder.Services.AddIntegrationsModule();
builder.Services.AddAgentDelivery();

// Browser authentication. There is no call to MapIdentityApi and no registration
// endpoint: an account exists only after an Invitation is accepted.
builder.Services
    .AddIdentity<Employee, IdentityRole<Guid>>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequiredLength = 12;
        options.Lockout.MaxFailedAccessAttempts = 10;
    })
    .AddEntityFrameworkStores<AccountsDbContext>()
    .AddDefaultTokenProviders();

// The browser talks to an API, so an unauthenticated call gets a status code
// rather than a redirect to a server-rendered sign-in page.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.SlidingExpiration = true;
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});

// Employee Agents authenticate with an Agent Token, never with the Employee's
// password, so the two schemes sit side by side.
builder.Services.AddAuthentication().AddAgentTokenAuthentication();

builder.Services.AddValidatorsFromAssemblyContaining<Program>(includeInternalTypes: true);
builder.Services.AddOpenApi();
builder.Services.AddHostedService<DatabaseMigrator>();

builder.Services
    .AddHealthChecks()
    .AddCheck<DatabaseReadinessHealthCheck>(
        DatabaseReadinessHealthCheck.Name,
        tags: [DatabaseReadinessHealthCheck.ReadyTag]);

var app = builder.Build();

app.UseSerilogRequestLogging();

// "The process is running." No dependency is consulted, so a database outage never
// makes the container look dead.
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });

// "The process can do its work." The database connection is checked.
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains(DatabaseReadinessHealthCheck.ReadyTag),
});

app.UseAuthentication();
app.UseAuthorization();

app.MapSessionEndpoints();
app.MapAgentDelivery();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// In production the backend serves the frontend's built files, so both run on one
// origin and no CORS configuration exists to get wrong. In development the Vite dev
// server serves the frontend and proxies /api and /mcp here.
if (Directory.Exists(app.Environment.WebRootPath))
{
    app.UseDefaultFiles();
    app.UseStaticFiles();
    app.MapFallbackToFile("index.html");
}

await app.RunAsync();

/// <summary>
/// Named so that the integration tests can boot this exact application through
/// <c>WebApplicationFactory</c>, with every module composed as in production.
/// </summary>
public partial class Program
{
    protected Program()
    {
    }
}
