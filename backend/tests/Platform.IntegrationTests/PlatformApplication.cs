using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Testcontainers.PostgreSql;

namespace Platform.IntegrationTests;

/// <summary>
/// The one test seam for the whole codebase: the composed application over HTTP,
/// backed by a real PostgreSQL 17.
/// </summary>
/// <remarks>
/// Every module is wired exactly as in production and nothing is replaced by a mock.
/// The rule that an Employee Agent's rights equal its Employee's rights only holds in
/// composition, so a test that swapped a module out could pass while the running
/// system was wrong.
/// <para>
/// The environment is Development, which is what makes the host apply each module's
/// migrations to the container at startup. A test therefore exercises the same
/// start-up path a developer's machine does.
/// </para>
/// <para>
/// Both entry points — the HTTP API and the MCP route — reach this same host, so
/// later tests for token scoping, Offboarding, and Invitation expiry need no new
/// harness.
/// </para>
/// </remarks>
public sealed class PlatformApplication : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _database = new PostgreSqlBuilder("postgres:17")
        .WithDatabase("platform")
        .Build();

    /// <summary>The container's connection string, for assertions about stored state.</summary>
    public string ConnectionString => _database.GetConnectionString();

    public async ValueTask InitializeAsync() => await _database.StartAsync(TestContext.Current.CancellationToken);

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _database.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.UseEnvironment(Environments.Development);
        builder.UseSetting("ConnectionStrings:Default", ConnectionString);
    }
}
