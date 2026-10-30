using Platform.SharedKernel.Identifiers;

namespace Platform.Projects.Domain;

/// <summary>
/// The link between an Employee and a Project. It opens the Repository Records of
/// every Repository in the Project, plus the Project Instruction.
/// </summary>
/// <remarks>
/// The Employee is held as an identifier, never as a foreign key into the
/// <c>accounts</c> schema. That is what makes a cross-module join impossible rather
/// than merely discouraged.
/// </remarks>
public sealed class ProjectMembership
{
    private ProjectMembership(Guid id, ProjectId projectId, EmployeeId employeeId, DateTimeOffset startedAt)
    {
        Id = id;
        ProjectId = projectId;
        EmployeeId = employeeId;
        StartedAt = startedAt;
    }

    private ProjectMembership()
    {
        // Entity Framework Core materialises through this constructor.
    }

    public Guid Id { get; private set; }

    public ProjectId ProjectId { get; private set; }

    public EmployeeId EmployeeId { get; private set; }

    public DateTimeOffset StartedAt { get; private set; }

    /// <summary>Set when Membership ends, including by Offboarding.</summary>
    public DateTimeOffset? EndedAt { get; private set; }

    public bool IsCurrent => EndedAt is null;

    public static ProjectMembership Start(ProjectId projectId, EmployeeId employeeId, DateTimeOffset startedAt) =>
        new(Guid.CreateVersion7(), projectId, employeeId, startedAt);

    public void End(DateTimeOffset endedAt) => EndedAt ??= endedAt;
}
