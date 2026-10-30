using System.ComponentModel;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using Platform.AgentDelivery.Authentication;
using Platform.Projects.Abstractions;
using Platform.Projects.Domain;

namespace Platform.AgentDelivery.Tools;

/// <summary>
/// The read-only tools an Employee Agent may call.
/// </summary>
/// <remarks>
/// No tool takes an Employee. Each one asks <see cref="ICurrentAgentEmployee"/> who
/// presented the Agent Token and passes that Employee to Projects, which answers
/// only through current Project Membership. Agent Delivery itself decides nothing
/// and stores nothing — it resolves the token, asks, and returns the answer.
/// </remarks>
[McpServerToolType]
public sealed class EmployeeProjectTools(IEmployeeProjectReader reader, ICurrentAgentEmployee currentEmployee)
{
    [McpServerTool(Name = "list_my_projects", ReadOnly = true)]
    [Description("Lists the Projects the signed-in Employee is currently a member of.")]
    public async Task<IReadOnlyList<AgentProjectView>> ListMyProjectsAsync(CancellationToken cancellationToken)
    {
        var projects = await reader.ListProjectsAsync(currentEmployee.RequireEmployeeId(), cancellationToken);

        return [.. projects.Select(project => new AgentProjectView(
            project.Id.ToString(),
            project.Name,
            project.Description))];
    }

    [McpServerTool(Name = "list_project_repositories", ReadOnly = true)]
    [Description("Lists the Repository Records of one Project the Employee is a member of.")]
    public async Task<IReadOnlyList<AgentRepositoryRecordView>> ListProjectRepositoriesAsync(
        [Description("The Project identifier from list_my_projects.")] string projectId,
        CancellationToken cancellationToken)
    {
        var parsed = ParseProjectId(projectId);
        var records = await reader.ListRepositoryRecordsAsync(currentEmployee.RequireEmployeeId(), parsed, cancellationToken);

        if (records.IsFailure)
        {
            throw new McpException(records.Error.First.Message);
        }

        return [.. records.Value.Select(record => new AgentRepositoryRecordView(
            record.Address.AbsoluteUri,
            record.Purpose))];
    }

    [McpServerTool(Name = "get_project_instruction", ReadOnly = true)]
    [Description("Returns the Instruction of one Project the Employee is a member of.")]
    public async Task<AgentInstructionView> GetProjectInstructionAsync(
        [Description("The Project identifier from list_my_projects.")] string projectId,
        CancellationToken cancellationToken)
    {
        var parsed = ParseProjectId(projectId);
        var instruction = await reader.GetInstructionAsync(currentEmployee.RequireEmployeeId(), parsed, cancellationToken);

        if (instruction.IsFailure)
        {
            throw new McpException(instruction.Error.First.Message);
        }

        return new AgentInstructionView(instruction.Value.Markdown, instruction.Value.UpdatedAt);
    }

    private static ProjectId ParseProjectId(string projectId) =>
        ProjectId.TryParse(projectId, out var parsed)
            ? parsed
            : throw new McpException($"'{projectId}' is not a Project identifier.");
}
