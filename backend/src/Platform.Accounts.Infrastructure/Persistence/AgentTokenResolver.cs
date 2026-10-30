using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Platform.Accounts.Abstractions;
using Platform.Accounts.Domain;
using Platform.SharedKernel.Errors;
using Platform.SharedKernel.Identifiers;

namespace Platform.Accounts.Infrastructure.Persistence;

/// <summary>
/// Looks a presented Agent Token up by its hash and reports whose it is.
/// </summary>
/// <remarks>
/// The presented secret is hashed here and never stored or logged. A revoked token
/// is treated exactly like an unknown one, so Offboarding takes effect the moment
/// the revocation is written.
/// </remarks>
internal sealed class AgentTokenResolver(AccountsDbContext context) : IAgentTokenResolver
{
    public async Task<Result<EmployeeId, ErrorList>> ResolveAsync(string secret, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(secret))
        {
            return AccountErrors.AgentTokenNotRecognised().ToErrorList();
        }

        var presented = SecretHash.Of(secret);

        var owner = await context.AgentTokens
            .AsNoTracking()
            .Where(token => token.TokenHash == presented && token.RevokedAt == null)
            .Select(token => (EmployeeId?)token.EmployeeId)
            .FirstOrDefaultAsync(cancellationToken);

        return owner is null
            ? Result.Failure<EmployeeId, ErrorList>(AccountErrors.AgentTokenNotRecognised().ToErrorList())
            : Result.Success<EmployeeId, ErrorList>(owner.Value);
    }
}
