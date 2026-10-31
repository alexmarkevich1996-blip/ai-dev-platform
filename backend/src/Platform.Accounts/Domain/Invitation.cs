using Platform.SharedKernel.Identifiers;

namespace Platform.Accounts.Domain;

/// <summary>
/// A named permission to create an account, carrying the first set of Project
/// Memberships. Accepting one is the only way an Employee account comes into being.
/// </summary>
/// <remarks>
/// The Project identifiers are held as-is, never as foreign keys into the
/// <c>projects</c> schema: that is what makes a cross-module join impossible rather
/// than merely discouraged, the same rule Project Membership applies to the Employee
/// identifier.
/// </remarks>
public sealed class Invitation
{
    /// <summary>An Invitation is valid for seven days — see <c>docs/technical-foundation.md</c>.</summary>
    public static readonly TimeSpan Lifetime = TimeSpan.FromDays(7);

    private readonly List<ProjectId> _projectIds;

    private Invitation(
        Guid id,
        string email,
        SecretHash tokenHash,
        IReadOnlyList<ProjectId> projectIds,
        DateTimeOffset issuedAt)
    {
        Id = id;
        Email = email;
        TokenHash = tokenHash;
        _projectIds = [.. projectIds];
        IssuedAt = issuedAt;
        ExpiresAt = issuedAt + Lifetime;
    }

    private Invitation()
    {
        // Entity Framework Core materialises through this constructor.
        Email = string.Empty;
        _projectIds = [];
    }

    public Guid Id { get; private set; }

    public string Email { get; private set; }

    public SecretHash TokenHash { get; private set; }

    /// <summary>
    /// The Projects this Invitation grants Membership to once accepted. May be empty:
    /// an Employee can be invited with no Project yet.
    /// </summary>
    public IReadOnlyList<ProjectId> ProjectIds => _projectIds;

    public DateTimeOffset IssuedAt { get; private set; }

    public DateTimeOffset ExpiresAt { get; private set; }

    /// <summary>Set once the Invitation is accepted.</summary>
    public DateTimeOffset? AcceptedAt { get; private set; }

    public static Invitation Issue(
        string email,
        string secret,
        IReadOnlyList<ProjectId> projectIds,
        DateTimeOffset issuedAt) =>
        new(Guid.CreateVersion7(), email, SecretHash.Of(secret), projectIds, issuedAt);

    public bool HasExpired(DateTimeOffset now) => now >= ExpiresAt;

    /// <summary>
    /// Marks the Invitation accepted, once. The use case that creates the Employee
    /// account and starts its first Memberships from <see cref="ProjectIds"/>
    /// belongs to the onboarding spec.
    /// </summary>
    public void MarkAccepted(DateTimeOffset acceptedAt) => AcceptedAt ??= acceptedAt;
}
