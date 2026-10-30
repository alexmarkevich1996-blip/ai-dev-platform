using Platform.SharedKernel.Identifiers;

namespace Platform.AgentDelivery.Authentication;

/// <summary>
/// The Employee whose Agent Token arrived with the current MCP request.
/// </summary>
/// <remarks>
/// MCP tools take this instead of an Employee parameter, so no tool can be asked to
/// answer as somebody else.
/// </remarks>
public interface ICurrentAgentEmployee
{
    /// <summary>
    /// The Employee the presented Agent Token belongs to. Throws when the request
    /// carries no Agent Token identity, which the MCP route's policy prevents.
    /// </summary>
    EmployeeId RequireEmployeeId();
}
