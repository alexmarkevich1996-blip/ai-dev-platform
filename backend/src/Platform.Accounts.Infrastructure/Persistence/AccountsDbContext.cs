using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Platform.Accounts.Domain;

namespace Platform.Accounts.Infrastructure.Persistence;

/// <summary>
/// The Accounts module's own database context and migration history.
/// </summary>
/// <remarks>
/// Everything it maps lives in the <c>accounts</c> schema. No other module's context
/// maps anything there, so a cross-module join cannot be written even by accident.
/// </remarks>
public sealed class AccountsDbContext(DbContextOptions<AccountsDbContext> options)
    : IdentityDbContext<Employee, IdentityRole<Guid>, Guid>(options)
{
    /// <summary>The schema this module owns.</summary>
    public const string Schema = "accounts";

    /// <summary>The table recording which migrations of this module have been applied.</summary>
    public const string MigrationHistoryTable = "__migrations_history";

    public DbSet<AgentToken> AgentTokens => Set<AgentToken>();

    public DbSet<Invitation> Invitations => Set<Invitation>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.OnModelCreating(builder);

        builder.HasDefaultSchema(Schema);
        builder.ApplyConfigurationsFromAssembly(typeof(AccountsDbContext).Assembly);

        // Identity names its own tables, which the snake-case convention leaves
        // alone. Renaming them here keeps one naming style across the schema and
        // states the vocabulary: these are Employees, not users.
        builder.Entity<IdentityRole<Guid>>().ToTable("roles");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("role_claims");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("employee_claims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("employee_logins");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("employee_roles");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("employee_tokens");
    }
}
