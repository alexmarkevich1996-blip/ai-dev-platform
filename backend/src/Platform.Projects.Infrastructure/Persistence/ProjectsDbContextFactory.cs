using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Platform.Projects.Infrastructure.Persistence;

/// <summary>
/// Lets <c>dotnet ef</c> build this context without starting the application.
/// </summary>
/// <remarks>
/// Adding a migration needs a model, not a database, so the connection string here
/// only has to parse. See AGENTS.md, "Add a database migration".
/// </remarks>
internal sealed class ProjectsDbContextFactory : IDesignTimeDbContextFactory<ProjectsDbContext>
{
    private const string DesignTimeConnectionString = "Host=localhost;Port=5432;Database=platform";

    public ProjectsDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__Default") ?? DesignTimeConnectionString;

        var options = new DbContextOptionsBuilder<ProjectsDbContext>()
            .UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable(
                ProjectsDbContext.MigrationHistoryTable,
                ProjectsDbContext.Schema))
            .UseSnakeCaseNamingConvention()
            .Options;

        return new ProjectsDbContext(options);
    }
}
