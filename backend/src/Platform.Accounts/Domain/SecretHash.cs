using System.Security.Cryptography;
using System.Text;

namespace Platform.Accounts.Domain;

/// <summary>
/// The stored form of a secret the platform hands out once and never shows again:
/// an Agent Token or an Invitation token.
/// </summary>
/// <remarks>
/// Hashing lives in one type so that no caller can decide to store a secret in the
/// clear. The secrets are long random strings rather than passwords, so a plain
/// SHA-256 digest is enough; there is nothing to brute-force.
/// </remarks>
public readonly record struct SecretHash
{
    /// <summary>A SHA-256 digest in lowercase hexadecimal is always 64 characters.</summary>
    public const int Length = 64;

    private SecretHash(string value) => Value = value;

    /// <summary>The lowercase hexadecimal SHA-256 digest of the secret.</summary>
    public string Value { get; }

    public static SecretHash Of(string secret)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(secret);

        var digest = SHA256.HashData(Encoding.UTF8.GetBytes(secret));
        return new SecretHash(Convert.ToHexStringLower(digest));
    }

    /// <summary>Rebuilds a hash already read from the database.</summary>
    public static SecretHash FromStoredValue(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return new SecretHash(value);
    }

    public override string ToString() => Value;
}
