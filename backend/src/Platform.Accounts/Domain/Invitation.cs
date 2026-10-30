namespace Platform.Accounts.Domain;

/// <summary>
/// A named permission to create an account. Accepting one is the only way an
/// Employee account comes into being.
/// </summary>
public sealed class Invitation
{
    /// <summary>An Invitation is valid for seven days — see <c>docs/technical-foundation.md</c>.</summary>
    public static readonly TimeSpan Lifetime = TimeSpan.FromDays(7);

    private Invitation(Guid id, string email, SecretHash tokenHash, DateTimeOffset issuedAt)
    {
        Id = id;
        Email = email;
        TokenHash = tokenHash;
        IssuedAt = issuedAt;
        ExpiresAt = issuedAt + Lifetime;
    }

    private Invitation()
    {
        // Entity Framework Core materialises through this constructor.
        Email = string.Empty;
    }

    public Guid Id { get; private set; }

    public string Email { get; private set; }

    public SecretHash TokenHash { get; private set; }

    public DateTimeOffset IssuedAt { get; private set; }

    public DateTimeOffset ExpiresAt { get; private set; }

    /// <summary>Set once the Invitation is accepted.</summary>
    public DateTimeOffset? AcceptedAt { get; private set; }

    public static Invitation Issue(string email, string secret, DateTimeOffset issuedAt) =>
        new(Guid.CreateVersion7(), email, SecretHash.Of(secret), issuedAt);

    public bool HasExpired(DateTimeOffset now) => now >= ExpiresAt;

    /// <summary>
    /// Marks the Invitation accepted, once. The use case that creates the Employee
    /// account and its first Memberships belongs to the onboarding spec.
    /// </summary>
    public void MarkAccepted(DateTimeOffset acceptedAt) => AcceptedAt ??= acceptedAt;
}
