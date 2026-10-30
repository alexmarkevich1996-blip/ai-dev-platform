using Platform.Projects.Domain;
using Platform.SharedKernel.Errors;

namespace Platform.Projects.Abstractions;

/// <summary>The failures the Projects module reports to callers outside it.</summary>
public static class ProjectErrors
{
    /// <summary>
    /// Returned both when the Project does not exist and when the Employee has no
    /// current Membership of it. One answer for both cases, so a caller cannot use
    /// the difference to discover which Projects exist.
    /// </summary>
    public static Error NotVisible(ProjectId projectId) => Error.NotFound(
        "project.not_visible",
        $"Project {projectId} does not exist, or this Employee is not a member of it.");

    public static Error InstructionNotWritten(ProjectId projectId) => Error.NotFound(
        "instruction.not_written",
        $"Project {projectId} has no Instruction yet.");
}
