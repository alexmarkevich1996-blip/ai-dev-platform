using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace Platform.Host.Health;

/// <summary>
/// Reports whether the database answers.
/// </summary>
/// <remarks>
/// This is the difference between <c>/health/live</c> and <c>/health/ready</c>: the
/// first says the process is running, this one says the process can do its work. It
/// opens an Npgsql connection and runs the cheapest possible statement, so it tests
/// the real connection path rather than a cached pool entry.
/// </remarks>
internal sealed class DatabaseReadinessHealthCheck(NpgsqlDataSource dataSource) : IHealthCheck
{
    /// <summary>The name this check reports under, and the tag selecting the readiness set.</summary>
    public const string Name = "database";

    public const string ReadyTag = "ready";

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var command = dataSource.CreateCommand("SELECT 1");
            _ = await command.ExecuteScalarAsync(cancellationToken);

            return HealthCheckResult.Healthy("The database answered.");
        }
        catch (NpgsqlException exception)
        {
            return HealthCheckResult.Unhealthy("The database did not answer.", exception);
        }
        catch (TimeoutException exception)
        {
            return HealthCheckResult.Unhealthy("The database did not answer in time.", exception);
        }
    }
}
