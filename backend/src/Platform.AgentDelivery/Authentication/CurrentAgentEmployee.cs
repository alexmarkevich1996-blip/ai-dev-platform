using Microsoft.AspNetCore.Http;
using Platform.SharedKernel.Identifiers;

namespace Platform.AgentDelivery.Authentication;

/// <summary>Reads the Employee from the claim the Agent Token handler issued.</summary>
internal sealed class CurrentAgentEmployee(IHttpContextAccessor httpContextAccessor) : ICurrentAgentEmployee
{
    public EmployeeId Require()
    {
        var claim = httpContextAccessor.HttpContext?.User.FindFirst(AgentTokenDefaults.EmployeeIdClaimType);

        if (claim is null || !EmployeeId.TryParse(claim.Value, out var employeeId))
        {
            // The MCP route requires the Agent Token policy, so reaching this point
            // means the route was mapped without it.
            throw new InvalidOperationException(
                "No Agent Token identity on this request. The MCP route must require the "
                + $"'{AgentTokenDefaults.Policy}' policy.");
        }

        return employeeId;
    }
}
