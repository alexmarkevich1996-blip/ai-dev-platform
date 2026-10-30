using CSharpFunctionalExtensions;
using Platform.SharedKernel.Errors;
using Platform.SharedKernel.Identifiers;

namespace Platform.Accounts.Abstractions;

/// <summary>
/// Turns the secret an Employee Agent presents into the Employee it belongs to.
/// </summary>
/// <remarks>
/// Accounts owns this because Accounts owns Agent Tokens. The caller hands over the
/// raw secret and gets back an Employee or a failure: hashing, lookup, and the
/// revocation check all stay inside, so no caller can skip one of them.
/// </remarks>
public interface IAgentTokenResolver
{
    Task<Result<EmployeeId, ErrorList>> ResolveAsync(string secret, CancellationToken cancellationToken);
}
