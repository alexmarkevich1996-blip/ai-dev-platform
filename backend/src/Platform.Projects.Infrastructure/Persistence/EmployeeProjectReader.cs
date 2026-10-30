using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Platform.Projects.Abstractions;
using Platform.Projects.Domain;
using Platform.SharedKernel.Errors;
using Platform.SharedKernel.Identifiers;

namespace Platform.Projects.Infrastructure.Persistence;

/// <summary>
/// Reads Project data through current Project Membership.
/// </summary>
/// <remarks>
/// Every query starts from the Membership table, so a Project the Employee has no
/// current Membership of is invisible rather than filtered out afterwards. An ended
/// Membership — including one ended by Offboarding — stops matching immediately.
/// </remarks>
internal sealed class EmployeeProjectReader(ProjectsDbContext context) : IEmployeeProjectReader
{
    public async Task<IReadOnlyList<ProjectSummary>> ListProjectsAsync(
        EmployeeId employeeId,
        CancellationToken cancellationToken)
    {
        return await CurrentMemberships(employeeId)
            .Join(
                context.Projects,
                membership => membership.ProjectId,
                project => project.Id,
                (_, project) => new ProjectSummary(project.Id, project.Name, project.Description))
            .OrderBy(summary => summary.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Result<IReadOnlyList<RepositoryRecordSummary>, ErrorList>> ListRepositoryRecordsAsync(
        EmployeeId employeeId,
        ProjectId projectId,
        CancellationToken cancellationToken)
    {
        if (!await IsMemberAsync(employeeId, projectId, cancellationToken))
        {
            return Result.Failure<IReadOnlyList<RepositoryRecordSummary>, ErrorList>(
                ProjectErrors.NotVisible(projectId).ToErrorList());
        }

        var records = await context.Projects
            .Where(project => project.Id == projectId)
            .SelectMany(project => project.RepositoryRecords)
            .OrderBy(record => record.AddedAt)
            .Select(record => new RepositoryRecordSummary(record.Id, record.Address, record.Purpose))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<RepositoryRecordSummary>, ErrorList>(records);
    }

    public async Task<Result<InstructionText, ErrorList>> GetInstructionAsync(
        EmployeeId employeeId,
        ProjectId projectId,
        CancellationToken cancellationToken)
    {
        if (!await IsMemberAsync(employeeId, projectId, cancellationToken))
        {
            return Result.Failure<InstructionText, ErrorList>(ProjectErrors.NotVisible(projectId).ToErrorList());
        }

        var instruction = await context.Projects
            .Where(project => project.Id == projectId && project.Instruction != null)
            .Select(project => new InstructionText(
                project.Id,
                project.Instruction!.Markdown,
                project.Instruction.UpdatedAt))
            .FirstOrDefaultAsync(cancellationToken);

        return instruction is null
            ? Result.Failure<InstructionText, ErrorList>(ProjectErrors.InstructionNotWritten(projectId).ToErrorList())
            : Result.Success<InstructionText, ErrorList>(instruction);
    }

    private IQueryable<ProjectMembership> CurrentMemberships(EmployeeId employeeId) =>
        context.ProjectMemberships
            .AsNoTracking()
            .Where(membership => membership.EmployeeId == employeeId && membership.EndedAt == null);

    private Task<bool> IsMemberAsync(EmployeeId employeeId, ProjectId projectId, CancellationToken cancellationToken) =>
        CurrentMemberships(employeeId).AnyAsync(membership => membership.ProjectId == projectId, cancellationToken);
}
