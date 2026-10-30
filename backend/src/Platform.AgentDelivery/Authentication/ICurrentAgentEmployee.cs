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
    EmployeeId Require();
}
