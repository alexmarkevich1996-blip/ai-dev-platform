using System.Globalization;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Platform.Accounts.Abstractions;

namespace Platform.AgentDelivery.Authentication;

/// <summary>
/// Turns the <c>Authorization: Bearer &lt;agent token&gt;</c> header into the Employee
/// the token belongs to.
/// </summary>
/// <remarks>
/// This is the one place an Employee Agent acquires an identity. It asks Accounts to
/// resolve the token and takes no other route: Agent Delivery stores nothing and has
/// no second source of truth to consult. Everything downstream then acts as that
/// Employee, which is how the rule "an Employee Agent's rights equal its Employee's
/// rights" holds without being restated in each tool.
/// </remarks>
internal sealed class AgentTokenAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory loggerFactory,
    UrlEncoder encoder,
    IAgentTokenResolver resolver)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, loggerFactory, encoder)
{
    private const string BearerPrefix = "Bearer ";

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var header = Request.Headers.Authorization.ToString();

        if (string.IsNullOrWhiteSpace(header))
        {
            // No header at all is "not our business", not a failure: another scheme
            // may answer the request.
            return AuthenticateResult.NoResult();
        }

        if (!header.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return AuthenticateResult.NoResult();
        }

        var secret = header[BearerPrefix.Length..].Trim();
        var resolved = await resolver.ResolveAsync(secret, Context.RequestAborted);

        if (resolved.IsFailure)
        {
            return AuthenticateResult.Fail(resolved.Error.First.Message);
        }

        var employeeId = resolved.Value.Value.ToString("D", CultureInfo.InvariantCulture);

        var identity = new ClaimsIdentity(AgentTokenDefaults.AuthenticationScheme);
        identity.AddClaim(new Claim(AgentTokenDefaults.EmployeeIdClaimType, employeeId, ClaimValueTypes.String));
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, employeeId));

        var principal = new ClaimsPrincipal(identity);
        return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.Headers[HeaderNames.WWWAuthenticate] = BearerPrefix.Trim();
        return base.HandleChallengeAsync(properties);
    }
}
