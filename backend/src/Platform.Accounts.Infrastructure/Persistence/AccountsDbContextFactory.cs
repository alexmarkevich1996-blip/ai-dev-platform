using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Platform.Accounts.Infrastructure.Persistence;

/// <summary>
/// Lets <c>dotnet ef</c> build this context without starting the application.
/// </summary>
/// <remarks>
/// Adding a migration needs a model, not a database, so the connection string here
/// only has to parse. See AGENTS.md, "Add a database migration".
/// </remarks>
internal sealed class AccountsDbContextFactory : IDesignTimeDbContextFactory<AccountsDbContext>
{
    /// <summary>
    /// Carries no credential on purpose: building the model needs a connection string
    /// that parses, not one that connects. Set <c>ConnectionStrings__Default</c> for
    /// commands that do reach the database, such as <c>dotnet ef database update</c>.
    /// </summary>
    private const string DesignTimeConnectionString = "Host=localhost;Port=5432;Database=platform";

    public AccountsDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__Default") ?? DesignTimeConnectionString;

        var options = new DbContextOptionsBuilder<AccountsDbContext>()
            .UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable(
                AccountsDbContext.MigrationHistoryTable,
                AccountsDbContext.Schema))
            .UseSnakeCaseNamingConvention()
            .Options;

        return new AccountsDbContext(options);
    }
}
