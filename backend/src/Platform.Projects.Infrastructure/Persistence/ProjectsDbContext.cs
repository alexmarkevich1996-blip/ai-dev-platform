using Microsoft.EntityFrameworkCore;
using Platform.Projects.Domain;

namespace Platform.Projects.Infrastructure.Persistence;

/// <summary>
/// The Projects module's own database context and migration history.
/// </summary>
/// <remarks>
/// Everything it maps lives in the <c>projects</c> schema. It maps no Employee, only
/// Employee identifiers, so there is no foreign key into <c>accounts</c> and no join
/// across the two modules is expressible.
/// </remarks>
public sealed class ProjectsDbContext(DbContextOptions<ProjectsDbContext> options) : DbContext(options)
{
    /// <summary>The schema this module owns.</summary>
    public const string Schema = "projects";

    /// <summary>The table recording which migrations of this module have been applied.</summary>
    public const string MigrationHistoryTable = "__migrations_history";

    public DbSet<Project> Projects => Set<Project>();

    public DbSet<ProjectMembership> ProjectMemberships => Set<ProjectMembership>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProjectsDbContext).Assembly);
    }
}
