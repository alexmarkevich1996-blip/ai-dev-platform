using CSharpFunctionalExtensions;
using Platform.Projects.Domain;
using Platform.SharedKernel.Errors;
using Platform.SharedKernel.Identifiers;

namespace Platform.Projects.Abstractions;

/// <summary>
/// Answers "what may this Employee see?" about Projects, Repository Records, and
/// Instructions.
/// </summary>
/// <remarks>
/// Every method takes the Employee asking. There is deliberately no "get Project by
/// identifier" on this interface: a caller cannot read Project data without naming
/// whose rights apply, so the Membership filter cannot be left out by accident.
/// This is the contract Agent Delivery uses to honour the rule that an Employee
/// Agent's rights equal its Employee's rights.
/// </remarks>
public interface IEmployeeProjectReader
{
    /// <summary>The Projects the Employee currently has Membership of.</summary>
    Task<IReadOnlyList<ProjectSummary>> ListProjectsAsync(EmployeeId employeeId, CancellationToken cancellationToken);

    /// <summary>
    /// The Repository Records of one Project, or a not-found failure when the
    /// Employee has no current Membership of it. A Project the Employee may not see
    /// is reported as missing rather than forbidden, so the answer leaks nothing.
    /// </summary>
    Task<Result<IReadOnlyList<RepositoryRecordSummary>, ErrorList>> ListRepositoryRecordsAsync(
        EmployeeId employeeId,
        ProjectId projectId,
        CancellationToken cancellationToken);

    /// <summary>The Instruction of one Project, under the same Membership condition.</summary>
    Task<Result<InstructionText, ErrorList>> GetInstructionAsync(
        EmployeeId employeeId,
        ProjectId projectId,
        CancellationToken cancellationToken);
}
