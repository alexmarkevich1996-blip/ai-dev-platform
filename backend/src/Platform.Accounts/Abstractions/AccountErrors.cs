using Platform.SharedKernel.Errors;

namespace Platform.Accounts.Abstractions;

/// <summary>The failures the Accounts module reports to callers outside it.</summary>
public static class AccountErrors
{
    /// <summary>
    /// One answer for an unknown token, a revoked token, and a malformed token.
    /// The caller learns only that the secret does not work.
    /// </summary>
    public static Error AgentTokenNotRecognised() => Error.Unauthorized(
        "agent_token.not_recognised",
        "This Agent Token is not recognised.");
}
