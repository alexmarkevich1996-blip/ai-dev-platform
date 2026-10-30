using System.Net;
using Npgsql;

namespace Platform.IntegrationTests;

/// <summary>
/// Proves the seam itself. If this fails, no other test in the suite can be trusted.
/// </summary>
[Collection(PlatformCollection.Name)]
public sealed class HarnessTests(PlatformApplication application)
{
    [Fact]
    public async Task The_composed_application_starts_migrates_both_schemas_and_reports_ready()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        // Creating the client starts the host. Startup applies each module's
        // migrations, so a broken migration fails here rather than silently later.
        using var client = application.CreateClient();

        var response = await client.GetAsync(new Uri("/health/ready", UriKind.Relative), cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", body);

        Assert.Equal(1, await AppliedMigrationCountAsync("accounts", cancellationToken));
        Assert.Equal(1, await AppliedMigrationCountAsync("projects", cancellationToken));
    }

    /// <summary>
    /// Counts the rows in one module's migration history. Each module keeps its own,
    /// in its own schema, so neither can report the other's state.
    /// </summary>
    private async Task<int> AppliedMigrationCountAsync(string schema, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(application.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = $"""SELECT count(*) FROM "{schema}"."__migrations_history";""";

        return Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));
    }
}
