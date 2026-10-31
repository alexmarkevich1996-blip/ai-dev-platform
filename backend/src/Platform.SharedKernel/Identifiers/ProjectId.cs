namespace Platform.SharedKernel.Identifiers;

/// <summary>
/// Identifies a Project across module boundaries.
/// </summary>
/// <remarks>
/// This type lives in the shared kernel, not in Projects, because an Invitation
/// carries the first set of Project Memberships to grant and Accounts must not
/// reference Projects. Keeping the identifier here is what lets the two modules
/// stay apart while still speaking about the same Project.
/// </remarks>
public readonly record struct ProjectId(Guid Value)
{
    public static ProjectId New() => new(Guid.CreateVersion7());

    public static bool TryParse(string? value, out ProjectId projectId)
    {
        if (Guid.TryParse(value, out var parsed))
        {
            projectId = new ProjectId(parsed);
            return true;
        }

        projectId = default;
        return false;
    }

    public override string ToString() => Value.ToString();
}
