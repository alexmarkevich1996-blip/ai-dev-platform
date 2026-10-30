using Platform.SharedKernel.Identifiers;

namespace Platform.Accounts.Domain;

/// <summary>
/// A secret an Employee issues and hands to their Employee Agent.
/// </summary>
/// <remarks>
/// Only the hash is stored, so a database leak exposes no working token. An Employee
/// holds several tokens at once, which is why Agent Tokens are a separate mechanism
/// from Identity: Offboarding must revoke all of them in one operation.
/// </remarks>
public sealed class AgentToken
{
    private AgentToken(Guid id, EmployeeId employeeId, string name, SecretHash tokenHash, DateTimeOffset issuedAt)
    {
        Id = id;
        EmployeeId = employeeId;
        Name = name;
        TokenHash = tokenHash;
        IssuedAt = issuedAt;
    }

    private AgentToken()
    {
        // Entity Framework Core materialises through this constructor.
        Name = string.Empty;
    }

    public Guid Id { get; private set; }

    public EmployeeId EmployeeId { get; private set; }

    /// <summary>What the Employee called this token, so they can tell theirs apart.</summary>
    public string Name { get; private set; }

    public SecretHash TokenHash { get; private set; }

    public DateTimeOffset IssuedAt { get; private set; }

    public DateTimeOffset? RevokedAt { get; private set; }

    public bool IsRevoked => RevokedAt is not null;

    public static AgentToken Issue(EmployeeId employeeId, string name, string secret, DateTimeOffset issuedAt) =>
        new(Guid.CreateVersion7(), employeeId, name, SecretHash.Of(secret), issuedAt);

    public void Revoke(DateTimeOffset revokedAt) => RevokedAt ??= revokedAt;
}
