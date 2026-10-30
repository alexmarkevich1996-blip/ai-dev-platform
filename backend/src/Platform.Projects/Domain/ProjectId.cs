namespace Platform.Projects.Domain;

/// <summary>Identifies a Project.</summary>
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
