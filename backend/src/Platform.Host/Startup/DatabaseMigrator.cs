using Microsoft.EntityFrameworkCore;
using Platform.Accounts.Infrastructure.Persistence;
using Platform.Projects.Infrastructure.Persistence;

namespace Platform.Host.Startup;

/// <summary>
/// Applies each module's migrations when the application starts in Development.
/// </summary>
/// <remarks>
/// A fresh clone reaches a working database without a manual step. Outside
/// Development nothing is applied: a separate migrator service is operations work and
/// is deferred — see <c>docs/technical-foundation.md</c>.
/// <para>
/// This runs as a hosted service rather than inline after <c>builder.Build()</c>, so
/// tools that only build the application — OpenAPI document generation, for one —
/// never touch a database.
/// </para>
/// </remarks>
internal sealed class DatabaseMigrator(
    IServiceScopeFactory scopeFactory,
    IHostEnvironment environment,
    ILogger<DatabaseMigrator> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!environment.IsDevelopment())
        {
            logger.LogInformation(
                "Environment is {Environment}; migrations are not applied automatically.",
                environment.EnvironmentName);
            return;
        }

        using var scope = scopeFactory.CreateScope();

        // One migration history per module, so the two never block each other.
        await scope.ServiceProvider.GetRequiredService<AccountsDbContext>().Database
            .MigrateAsync(cancellationToken);
        await scope.ServiceProvider.GetRequiredService<ProjectsDbContext>().Database
            .MigrateAsync(cancellationToken);

        logger.LogInformation("Migrations of the accounts and projects schemas are up to date.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
