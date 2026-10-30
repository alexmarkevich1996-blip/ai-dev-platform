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
    }
}
